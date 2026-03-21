using UnityEngine;
using System;
using UnityEngine.Rendering;

[Serializable]
[VolumeComponentMenu("Custom/SphereVolumeComponent")]
public class SphereVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter intensity = new ClampedFloatParameter(value: 0f, min: 0f, max: 1f, overrideState: true);
    
    //tells when our effect should be rendered
    public bool IsActive() => intensity.value > 0f;
}
