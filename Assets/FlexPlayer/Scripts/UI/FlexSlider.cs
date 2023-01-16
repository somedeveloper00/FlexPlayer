using AnimFlex.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlexPlayer.UI
{
	public class FlexSlider : Slider
	{
		const float handleMinimizedSize = 0.2f;
		const float handleMaximizedSize = 1f;
		const float resizeDuration = 0.3f;

		Tweener _resizeTweener;

		protected override void Start() {
			base.Start();
			_resizeTweener = handleRect.AnimScaleTo( Vector3.one * handleMinimizedSize, Ease.OutQuad, resizeDuration );
		}

		public override void OnPointerDown(PointerEventData eventData) {
			base.OnPointerDown( eventData );
			if ( _resizeTweener.IsValid() ) _resizeTweener.Kill( false, false );
			_resizeTweener = handleRect.AnimScaleTo( Vector3.one * handleMaximizedSize, Ease.OutQuad, resizeDuration );
		}
		
		public override void OnPointerUp(PointerEventData eventData) {
			base.OnPointerUp( eventData );
			if ( _resizeTweener.IsValid() ) _resizeTweener.Kill( false, false );
			_resizeTweener = handleRect.AnimScaleTo( Vector3.one * handleMinimizedSize, Ease.OutQuad, resizeDuration );
		}
	}
}