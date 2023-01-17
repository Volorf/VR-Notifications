using System;
using TMPro;
using UnityEngine;

namespace Volorf.VRNotifications
{
    public class UpdateElementsSize : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private TextMeshProUGUI NotificationLabelText;
        [SerializeField] private RectTransform NotificationLabelRect;
        [SerializeField] private RectTransform NotificationCanvasRect;

        [Header("Settings")] 
        [SerializeField] private float VerticalOffset = 0.08f;
        [SerializeField] private float HorizontalOffset = 0.08f;

        [SerializeField] private float MinWidth = 0.24f;
        [SerializeField] private float MinHeight = 0.24f;
        

        private Vector2 _labelSize;

        private void OnDrawGizmosSelected()
        {
            UpdateSizeOfElements();
        }

        private void UpdateLabelSize()
        {
            _labelSize.x = NotificationLabelText.renderedWidth;
            _labelSize.y = NotificationLabelText.renderedHeight;
        }

        public void UpdateSizeOfElements()
        {
            UpdateLabelSize();
            float finalWidth = _labelSize.x + HorizontalOffset;
            float finalHeight = _labelSize.y + VerticalOffset;

            if (finalWidth <= 0) finalWidth = MinWidth;
            if (finalHeight <= 0) finalHeight = MinHeight;
    
            Vector2 finalSize = new Vector2(finalWidth, finalHeight);
            NotificationCanvasRect.sizeDelta = finalSize;
            // NotificationLabelRect.sizeDelta = finalSize;
            // print(finalSize);
        }
    }
}
