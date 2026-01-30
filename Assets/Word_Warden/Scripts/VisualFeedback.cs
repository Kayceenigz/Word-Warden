using UnityEngine;
using DG.Tweening; // Needs DOTween package

public class VisualFeedback : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Material glitchMaskMaterial;
    public Material standardMaterial;

    public void PlayUnmaskEffect()
    {
        // GDD: "Unmask peel (shader)"
        // Flash white then swap materials
        spriteRenderer.DOColor(Color.white, 0.1f).OnComplete(() => {
            spriteRenderer.material = standardMaterial;
            spriteRenderer.DOColor(Color.white, 0f);
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
        });
    }

    public void PlayRecruitAnimation()
    {
        // GDD: "Glow effect + kneel pose"
        transform.DOKill(); // Stop movement
        spriteRenderer.DOColor(Color.cyan, 0.5f).SetLoops(-1, LoopType.Yoyo);
        transform.DOLocalMoveY(transform.position.y - 0.5f, 0.3f); // "Kneel"
    }
}