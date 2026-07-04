using Core.ServiceLocatorDI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.Tools
{
    public sealed class UIPopupShower : MonoBehaviour, IService
    {
        [SerializeField] private Image _colorImage;
        
        private Tween _colorTween;
        
        public void Construct()
        {
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister(this);
        }
        
        public void ShowColor(Color color)
        {
            _colorImage.color = new Color(color.r, color.g, color.b, 0.5f);
            
            _colorTween?.Kill();
            _colorTween = _colorImage.DOFade(0, 0.3f).SetLoops(2, LoopType.Yoyo);
            _colorTween.OnComplete(() => { _colorTween = _colorImage.DOFade(0, 0.2f); });
        }
    }
}