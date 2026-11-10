# Hill Climb 3D

A mobile-first 3D hill driving prototype built with Three.js.

## Play
Once GitHub Pages is enabled and the deployment workflow completes, open:
https://pankaj7576.github.io/itz_pankaj_020/

## Mobile controls
- GAS: accelerate
- BRAKE: slow down / reverse
- TILT: rotate while airborne
- Restart: reset the run

## Tech
- Three.js loaded from jsDelivr CDN
- Responsive touch controls
- Procedural rolling terrain, jeep, coins, distance and fuel HUD

## Publish
This repository includes a GitHub Actions workflow at `.github/workflows/deploy-pages.yml`.
In repository Settings → Pages, set Build and deployment → Source to **GitHub Actions** once. The workflow then deploys on every push to `main`.

This is a browser prototype, not an Android APK. A native APK would require a separate Android packaging/build pipeline.
