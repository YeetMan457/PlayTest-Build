using LitMotion;
using LitMotion.Animation;
using LitMotion.Extensions;
using UnityEngine;

public class MenuOpenAnimation : MonoBehaviour
{
    public MotionHandle animation;
    public Vector3 startScale = new Vector3(0, 1);
    public Vector3 endScale = Vector3.one;
    public float duration = 0.5f;
    public Ease ease = Ease.OutBack;

    //public void PlayOpenAnimation()
    //{
    //    if (animation.IsActive())
    //        animation.Cancel();
    //    animation = LMotion.Create(startScale, endScale, duration)
    //        .WithEase(ease)
    //        .WithLoops(1, LoopType.Restart)
    //        .BindToLocalScale(transform);
    //}

    private void OnEnable()
    {
        if (animation.IsActive())
            animation.Cancel();
        animation = LMotion.Create(startScale, endScale, duration)
            .WithEase(ease)
            .WithLoops(1, LoopType.Restart)
            .BindToLocalScale(transform);
    }


}
