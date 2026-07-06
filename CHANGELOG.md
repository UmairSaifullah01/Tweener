# Changelog

All notable changes to this project will be documented in this file.

## [2.0.0] - 2025-02-22

### Fixed
- **RotateTowards / RotateTowards2**: Rotation now uses `Quaternion.Slerp` over time instead of `Quaternion.RotateTowards` with an incorrect parameter, so rotation completes correctly over the given duration.
- **Tween.Lerp()**: The internal float tween created by `Lerp()` no longer wires its completion to the calling tween instance; completion events now only fire when the main tween completes.
- **Flash eases**: Implemented `Flash`, `InFlash`, `OutFlash`, and `InOutFlash` in `EaseManager` (previously fell through to Linear).

### Changed
- **SmoothFollow**: Documented that the tween runs until `Kill()` is called (no fixed duration); `smoothTime` controls follow responsiveness.
- **ResumesAll** renamed to **ResumeAll** in `TweenCore` for consistency with `PauseAll` and `PlayAll`.
- **README**: Updated namespace to `THEBADDEST.Tweening2`, corrected Quick Start example and API description.

### Added
- **Tween-returning extension methods**: `MoveTween`, `MoveLocalTween`, `ScaleTween`, `RotateTween`, and `RotateLocalTween` on `Transform` return the underlying `Tween` so sequences can use them without allocating a `TweenerWrapper`.
