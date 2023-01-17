using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Volorf.VRNotifications
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance
        {
            get
            {
                return _notificationManager;
            }
        }

        private static NotificationManager _notificationManager;

        [Header("Settings")]
        [SerializeField] private NotificationSettings NotificationSettings;

        [Header("Elements")]
        [SerializeField] private UpdateElementsSize UpdateElementsSizeInstance;
        [SerializeField] private Canvas UICanvas;
        [SerializeField] private Image BackgroundImage;
        [SerializeField] private TextMeshProUGUI MessageLabel;
        
        private Transform _camera;
        private Vector3 _smoothPositionVelocity = Vector3.zero;
        private Vector3 _smoothForwardVelocity = Vector3.zero;
        private Queue<Notification> _notificationQueue = new Queue<Notification>();
        private bool _isNotificationExecutorRunning = false;
        private bool _isMessageShowing = false;
        private int _messageCounter = 0;
        private bool _canUpdateSizeOfElements;
        
        private void Awake()
        {
            if (_notificationManager != null && _notificationManager != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _notificationManager = this;
            }
        }
        
        private void Start()
        {
            if (Camera.main != null) _camera = Camera.main.transform;
            UICanvas.transform.localScale = Vector3.zero;
        }

        [ContextMenu("Send Debug Message")]
        public void SendDebugMessage()
        {
            _messageCounter += 1;
            string message = "Message #" + _messageCounter;
            Notification not = new Notification(message, NotificationType.Info);
            SendMessage(not); 
        }

        public void SendMessage(string message, NotificationType type)
        {
            Notification notification = new Notification(message, type);
            AddMessageToQueue(notification);
        }

        public void SendMessage(string message)
        {
            Notification notification = new Notification(message, NotificationType.Info);
            AddMessageToQueue(notification);
        }

        public void SendMessage(Notification not)
        {
            AddMessageToQueue(not);
        }

        private void AddMessageToQueue(Notification not)
        {
            _notificationQueue.Enqueue(not);

            if (!_isNotificationExecutorRunning)
            {
                StartCoroutine(ExecuteNotifications());
                
                if (NotificationSettings.FollowHead)
                {
                    StartCoroutine(FollowHead());
                }
                
            }
        }
        
        private void ShowMessage(Notification notification)
        {
            //Prepare elements
            switch (notification.Type)
            {
                case NotificationType.Info:
                    BackgroundImage.color = NotificationSettings.defaultNotificationStyle.backgroundColor;
                    MessageLabel.color = NotificationSettings.defaultNotificationStyle.messageColor;
                    break;
                case NotificationType.Warning:
                    BackgroundImage.color = NotificationSettings.warningNotificationStyle.backgroundColor;
                    MessageLabel.color = NotificationSettings.warningNotificationStyle.messageColor;
                    break;
                case NotificationType.Error:
                    BackgroundImage.color = NotificationSettings.errorNotificationStyle.backgroundColor;
                    MessageLabel.color = NotificationSettings.errorNotificationStyle.messageColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            transform.position = CalculateSnackBarPosition();
            transform.forward = (_camera.position - transform.position).normalized;
            
            TurnOnElements();
            
            UICanvas.transform.localScale = Vector3.zero;
            MessageLabel.text = notification.Message;

            StartCoroutine(CallNextFrame());
            
            Action callback = delegate { HideMessage(notification); };
            StartCoroutine(MessageAnimation(
                Vector3.one,
                NotificationSettings.toShowDuration, 
                0f,
                NotificationSettings.showCurve, 
                callback));
        }

        private void HideMessage(Notification notification)
        {
            Action callback = delegate { TurnOffElements(); };
            StartCoroutine(MessageAnimation(
                Vector3.zero, 
                NotificationSettings.toHideDuration,
                NotificationSettings.defaultDuration, 
                NotificationSettings.hideCurve, 
                callback));
        }

        private void TurnOnElements()
        {
            _isMessageShowing = true;
            SetElementsState(true);
        }

        private void TurnOffElements()
        {
            _isMessageShowing = false;
            SetElementsState(false);
        }

        private void SetElementsState(bool b)
        {
            UICanvas.enabled = b;
            // messageLabel.enabled = b;
        }
        
        private Vector3 CalculateSnackBarPosition()
        {
            Vector3 forwardDir;

            if (NotificationSettings.IsOffsetRelative)
            {
                forwardDir = _camera.forward;
            }
            else
            {
                forwardDir =  new Vector3(_camera.forward.x, 0f, _camera.forward.z).normalized;
            }
            
            return _camera.position + forwardDir * NotificationSettings.distanceFromHead + _camera.up * -1f * NotificationSettings.downOffset;
        }

        private IEnumerator ExecuteNotifications()
        {
            _isNotificationExecutorRunning = true;
            
            while (_notificationQueue.Count > 0)
            {
                if (!_isMessageShowing)
                {
                    Notification messageToShow = _notificationQueue.Dequeue();
                    ShowMessage(messageToShow);
                }
                
                yield return new WaitForSeconds(NotificationSettings.checkingFrequency);
            }
            
            _isNotificationExecutorRunning = false;
        }

        private IEnumerator CallNextFrame()
        {
            yield return null;
            UpdateElementsSizeInstance.UpdateSizeOfElements();
        }

        private IEnumerator FollowHead()
        {
            while (_isNotificationExecutorRunning || _isMessageShowing)
            {
                Vector3 newPos = CalculateSnackBarPosition();
                transform.position = Vector3.SmoothDamp(transform.position, newPos,
                    ref _smoothPositionVelocity, NotificationSettings.followHeadSmoothKoef);

                Vector3 newForward = (_camera.position - transform.position).normalized;
                transform.forward = Vector3.SmoothDamp(transform.forward, newForward, ref _smoothForwardVelocity, NotificationSettings.lookAtHeadSmoothKoef);
                
                yield return null;
            }
        }

        private IEnumerator MessageAnimation(Vector3 targetScale, float duration, float delay, AnimationCurve curve, Action callback)
        {
            float timer = 0f;
            Vector3 curScale = UICanvas.transform.localScale;

            while (timer <= delay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0f;
            
            while (timer <= duration)
            {
                float val = timer / duration;
                Vector3 newScale = Vector3.LerpUnclamped(curScale, targetScale, curve.Evaluate(val));
                UICanvas.transform.localScale = newScale;
                timer += Time.deltaTime;
                yield return null;
            }
            
            callback();
        }
    }
}