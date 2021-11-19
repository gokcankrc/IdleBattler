using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer healthSpriteRenderer;
    [SerializeField] private Transform healthSpritePivot;
    
    [SerializeField] private Color highHealthColor;
    [SerializeField] private Color lowHealthColor;
    
    private Vector2 _initialHealthBarSize;
    private Vector2 _currentHealthBarSize;

    private float lowHealthTreshold = 0.4f;
    
    // StartByCharacter is called by Character.cs
    public void Start()
    {
        _initialHealthBarSize = healthSpritePivot.localScale;
        _currentHealthBarSize = _initialHealthBarSize;
    
        // todo; this will cause a bug in the future.
        // happens when you enter a battle with low health.
        healthSpriteRenderer.color = highHealthColor;
    }

    // Update is called once per frame
    public void UpdateHealthBarSize(float fraction)
    {
        // update color
        if (fraction < lowHealthTreshold) healthSpriteRenderer.color = lowHealthColor;
        else healthSpriteRenderer.color = highHealthColor;
        
        // update scale
        _currentHealthBarSize = new Vector2(_initialHealthBarSize.x * fraction, _initialHealthBarSize.y);
        if (fraction < 0) _currentHealthBarSize = Vector2.zero;
        
        healthSpritePivot.localScale = _currentHealthBarSize;
    }
}
