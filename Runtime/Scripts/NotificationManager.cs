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
        [SerializeField] private NotificationSettings notificationSettings;

        [Header("Elements")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI messageLabel;
        private Transform _camera;
        
        private Vector3 _smoothPositionVelocity = Vector3.zero;
        private Vector3 _smoothForwardVelocity = Vector3.zero;

        private Queue<Notification> _notificationQueue = new Queue<Notification>();
        private bool _isNotificationExecutorRunning = false;
        private bool _isMessageShowing = false;

        private int _messageCounter = 0;

        [ContextMenu("Show Message")]
        public void ShowMessage()
        {
            _messageCounter++;
            Notification not = new Notification("Message number " + _messageCounter, NotificationType.Error, 5f);
            AddMessage(not); 
        }

        public void AddMessage(Notification not)
        {
            _notificationQueue.Enqueue(not);
            print("Length of Notification Queue: " + _notificationQueue.Count);
            
            if (!_isNotificationExecutorRunning)
            {
                StartCoroutine(ExecuteNotifications());
                StartCoroutine(FollowHead());
            }
        }
        
        
        private void ShowMessage(Notification notification)
        {
            //Prepare elements
            switch (notification.Type)
            {
                case NotificationType.Info:
                    backgroundImage.color = notificationSettings.defaultNotificationStyle.backgroundColor;
                    messageLabel.color = notificationSettings.defaultNotificationStyle.messageColor;
                    break;
                case NotificationType.Warning:
                    backgroundImage.color = notificationSettings.warningNotificationStyle.backgroundColor;
                    messageLabel.color = notificationSettings.warningNotificationStyle.messageColor;
                    break;
                case NotificationType.Error:
                    backgroundImage.color = notificationSettings.errorNotificationStyle.backgroundColor;
                    messageLabel.color = notificationSettings.errorNotificationStyle.messageColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            transform.position = CalculateSnackBarPosition();
            transform.forward = (_camera.position - transform.position).normalized;
            
            TurnOnElements();
            
            canvas.transform.localScale = Vector3.zero;
            messageLabel.text = notification.Message;
            
            Action callback = delegate { HideMessage(notification); };
            StartCoroutine(MessageAnimation(
                Vector3.one,
                notificationSettings.toShowDuration, 
                0f,
                notificationSettings.showCurve, 
                callback));
        }

        private void HideMessage(Notification notification)
        {
            Action callback = delegate { TurnOffElements(); };
            StartCoroutine(MessageAnimation(
                Vector3.zero, 
                notificationSettings.toHideDuration,
                notificationSettings.defaultDuration, 
                notificationSettings.hideCurve, 
                callback));
        }
        
        private void Start()
        {
            if (Camera.main != null) _camera = Camera.main.transform;
            canvas.transform.localScale = Vector3.zero;
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
            canvas.enabled = b;
            // messageLabel.enabled = b;
        }
        
        private Vector3 CalculateSnackBarPosition()
        {
            return _camera.position + _camera.forward * notificationSettings.distanceFromHead + _camera.up * -1f * notificationSettings.downOffset;
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
                
                yield return new WaitForSeconds(notificationSettings.checkingFrequency);
            }
            
            _isNotificationExecutorRunning = false;
        }

        private IEnumerator FollowHead()
        {
            while (_isNotificationExecutorRunning || _isMessageShowing)
            {
                Vector3 newPos = CalculateSnackBarPosition();
                transform.position = Vector3.SmoothDamp(transform.position, newPos,
                    ref _smoothPositionVelocity, notificationSettings.followHeadSmoothKoef);

                Vector3 newForward = (_camera.position - transform.position).normalized;
                transform.forward = Vector3.SmoothDamp(transform.forward, newForward, ref _smoothForwardVelocity, notificationSettings.lookAtHeadSmoothKoef);
                
                yield return null;
            }
        }

        private IEnumerator MessageAnimation(Vector3 targetScale, float duration, float delay, AnimationCurve curve, Action callback)
        {
            float timer = 0f;
            Vector3 curScale = canvas.transform.localScale;

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
                canvas.transform.localScale = newScale;
                timer += Time.deltaTime;
                yield return null;
            }
            
            callback();
        }
    }
}
