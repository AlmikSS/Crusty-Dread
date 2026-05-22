using Core.ServiceLocatorDI;
using Core.TicksSystem;
using UnityEngine;

namespace GamePlay.Props
{
    public class FishingRopLine : MonoBehaviour
    {
        [SerializeField] private Transform _ropTipTransform;
        [SerializeField] private Transform _hookTransform;
        [SerializeField] private LineRenderer _lineRenderer;
        
        private void Start()
        {
            if (_lineRenderer == null)
            {
                Debug.LogError("FishingRopLine: _lineRenderer == null");
            }
            _lineRenderer.positionCount = 2;
        }

        private void Update()
        {
            if (_lineRenderer == null)
                return;
            
            _lineRenderer.SetPosition(0, _ropTipTransform.position);
            _lineRenderer.SetPosition(1, _hookTransform.position);
        }
    }
}