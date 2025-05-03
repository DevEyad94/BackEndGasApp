export default `<!doctype html>
<html lang="en" data-beasties-container="">
<head>
  <meta charset="utf-8">
  <title>GraveNew</title>
  <base href="/">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="icon" type="image/x-icon" href="assets/logo/favicon.ico">
  <link rel="preconnect" href="https://fonts.googleapis.com">
  <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="">
  <link href="https://fonts.googleapis.com/css2?family=Tajawal:wght@400;500;700&amp;display=swap" rel="stylesheet">
  <!-- Font Awesome for icons -->
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">

  <!-- <script src="https://aframe.io/releases/1.3.0/aframe.min.js"></script>

  <script src="https://raw.githack.com/AR-js-org/AR.js/3.4.5/three.js/build/ar-threex-location-only.js"></script>
  <script src="https://raw.githack.com/AR-js-org/AR.js/3.4.5/aframe/build/aframe-ar.js"></script> -->


  <!-- <script src="https://aframe.io/releases/1.6.0/aframe.min.js"></script>
  <script src="https://unpkg.com/aframe-look-at-component@1.0.0/dist/aframe-look-at-component.min.js"></script>
  <script src="https://raw.githack.com/AR-js-org/AR.js/master/aframe/build/aframe-ar-nft.js"></script> -->

  <script src="https://aframe.io/releases/1.6.0/aframe.min.js"></script>
  <script src="https://unpkg.com/aframe-look-at-component@1.0.0/dist/aframe-look-at-component.min.js"></script>
  <script src="https://raw.githack.com/AR-js-org/AR.js/master/aframe/build/aframe-ar-nft.js"></script>
  <!-- Load look-at component -->
  <!-- <script src="https://unpkg.com/aframe-look-at-component@1.0.0/dist/aframe-look-at-component.min.js"></script> -->
  <!-- Load AR.js with GPS support -->
  <!-- <script src="https://raw.githack.com/AR-js-org/AR.js/master/aframe/build/aframe-ar.js"></script> -->

  <!-- AR cleanup helper script -->
  <script>
    // Create global AR cleanup helper
    window.ARCleanup = {
      // Store references to active media streams
      activeStreams: [],

      // Function to register a stream for later cleanup
      registerStream: function(stream) {
        if (stream && stream.getTracks && stream.getTracks().length > 0) {
          this.activeStreams.push(stream);
        }
      },

      // Function to clean up all registered streams
      cleanup: function() {
        // Stop all registered media streams
        this.activeStreams.forEach(stream => {
          if (stream && stream.getTracks) {
            stream.getTracks().forEach(track => {
              if (track && track.stop) {
                track.stop();
              }
            });
          }
        });

        // Clear the array
        this.activeStreams = [];

        // Force garbage collection on any a-scene elements
        const scenes = document.querySelectorAll('a-scene');
        scenes.forEach(scene => {
          if (scene && scene.parentNode) {
            // Make a backup of the node
            const parent = scene.parentNode;
            const next = scene.nextSibling;
            // Remove it from DOM to force cleanup
            parent.removeChild(scene);
            // If this is just a temporary removal, you could reattach it here
            // (but we're not doing that in this case)
          }
        });

        // Clean up any video elements
        const videos = document.querySelectorAll('video');
        videos.forEach(video => {
          if (video.srcObject) {
            video.srcObject = null;
          }
          if (video.src) {
            video.src = '';
            video.load();
          }
        });

        // Return true to indicate successful cleanup
        return true;
      }
    };

    // Patch getUserMedia to track streams
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
      const originalGetUserMedia = navigator.mediaDevices.getUserMedia;
      navigator.mediaDevices.getUserMedia = function() {
        return originalGetUserMedia.apply(this, arguments)
          .then(stream => {
            window.ARCleanup.registerStream(stream);
            return stream;
          });
      };
    }
  </script>
<style>@layer theme{:root{--font-sans:ui-sans-serif, system-ui, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji";--font-mono:ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", "Courier New", monospace;--color-red-50:oklch(.971 .013 17.38);--color-red-200:oklch(.885 .062 18.334);--color-red-500:oklch(.637 .237 25.331);--color-red-600:oklch(.577 .245 27.325);--color-red-700:oklch(.505 .213 27.518);--color-red-800:oklch(.444 .177 26.899);--color-yellow-50:oklch(.987 .026 102.212);--color-yellow-200:oklch(.945 .129 101.54);--color-yellow-500:oklch(.795 .184 86.047);--color-yellow-700:oklch(.554 .135 66.442);--color-yellow-900:oklch(.421 .095 57.708);--color-green-50:oklch(.982 .018 155.826);--color-green-200:oklch(.925 .084 155.995);--color-green-500:oklch(.723 .219 149.579);--color-green-600:oklch(.627 .194 149.214);--color-green-700:oklch(.527 .154 150.069);--color-green-800:oklch(.448 .119 151.328);--color-emerald-100:oklch(.95 .052 163.051);--color-emerald-300:oklch(.845 .143 164.978);--color-emerald-400:oklch(.765 .177 163.223);--color-emerald-600:oklch(.596 .145 163.225);--color-emerald-700:oklch(.508 .118 165.612);--color-emerald-800:oklch(.432 .095 166.913);--color-teal-500:oklch(.704 .14 182.503);--color-blue-500:oklch(.623 .214 259.815);--color-blue-600:oklch(.546 .245 262.881);--color-indigo-50:oklch(.962 .018 272.314);--color-purple-500:oklch(.627 .265 303.9);--color-purple-700:oklch(.496 .265 301.924);--color-gray-50:oklch(.985 .002 247.839);--color-gray-100:oklch(.967 .003 264.542);--color-gray-200:oklch(.928 .006 264.531);--color-gray-300:oklch(.872 .01 258.338);--color-gray-400:oklch(.707 .022 261.325);--color-gray-500:oklch(.551 .027 264.364);--color-gray-600:oklch(.446 .03 256.802);--color-gray-700:oklch(.373 .034 259.733);--color-gray-800:oklch(.278 .033 256.848);--color-gray-900:oklch(.21 .034 264.665);--color-black:#000;--color-white:#fff;--spacing:.25rem;--container-sm:24rem;--container-md:28rem;--container-lg:32rem;--container-2xl:42rem;--container-3xl:48rem;--container-4xl:56rem;--container-5xl:64rem;--container-7xl:80rem;--text-xs:.75rem;--text-xs--line-height:calc(1 / .75);--text-sm:.875rem;--text-sm--line-height:calc(1.25 / .875);--text-base:1rem;--text-base--line-height:1.5 ;--text-lg:1.125rem;--text-lg--line-height:calc(1.75 / 1.125);--text-xl:1.25rem;--text-xl--line-height:calc(1.75 / 1.25);--text-2xl:1.5rem;--text-2xl--line-height:calc(2 / 1.5);--text-3xl:1.875rem;--text-3xl--line-height:1.2 ;--text-4xl:2.25rem;--text-4xl--line-height:calc(2.5 / 2.25);--text-5xl:3rem;--text-5xl--line-height:1;--text-6xl:3.75rem;--text-6xl--line-height:1;--font-weight-medium:500;--font-weight-semibold:600;--font-weight-bold:700;--font-weight-extrabold:800;--tracking-tight:-.025em;--leading-relaxed:1.625;--radius-md:.375rem;--radius-lg:.5rem;--radius-xl:.75rem;--ease-in-out:cubic-bezier(.4, 0, .2, 1);--animate-spin:spin 1s linear infinite;--blur-sm:8px;--blur-3xl:64px;--default-transition-duration:.15s;--default-transition-timing-function:cubic-bezier(.4, 0, .2, 1);--default-font-family:var(--font-sans);--default-font-feature-settings:var(--font-sans--font-feature-settings);--default-font-variation-settings:var( --font-sans--font-variation-settings );--default-mono-font-family:var(--font-mono);--default-mono-font-feature-settings:var( --font-mono--font-feature-settings );--default-mono-font-variation-settings:var( --font-mono--font-variation-settings )}}@layer base{*,:after,:before{box-sizing:border-box;margin:0;padding:0;border:0 solid}html{line-height:1.5;-webkit-text-size-adjust:100%;tab-size:4;font-family:var(--default-font-family, ui-sans-serif, system-ui, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji");font-feature-settings:var(--default-font-feature-settings, normal);font-variation-settings:var(--default-font-variation-settings, normal);-webkit-tap-highlight-color:transparent}body{line-height:inherit}}@layer theme{:root{--arabic-black:#000000;--arabic-green:#1F8A46;--arabic-green-light:#25A152;--arabic-green-dark:#176835;--arabic-white:#FFFFFF;--emerald-300:#6ee7b7;--emerald-400:#34d399;--teal-500:#14b8a6;--font-tajawal:"Tajawal", sans-serif;--gray-50:#F9FAFB;--gray-100:#F3F4F6;--gray-200:#E5E7EB;--gray-300:#D1D5DB;--gray-400:#9CA3AF;--gray-500:#6B7280;--gray-600:#4B5563;--gray-700:#374151;--gray-800:#1F2937;--gray-900:#111827;--tw-translate-x:0;--tw-translate-y:0;--tw-rotate:0;--tw-skew-x:0;--tw-skew-y:0;--tw-scale-x:1;--tw-scale-y:1;--tw-blur:0;--tw-gradient-from-position:0%;--tw-gradient-to-position:100%}}@layer utilities{}@layer base{html{color-scheme:light dark}body{background-color:var(--color-white);color:var(--color-gray-900);font-family:var(--font-tajawal),system-ui,-apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,Oxygen,Ubuntu,Cantarell,"Open Sans","Helvetica Neue",sans-serif}@media (prefers-color-scheme: dark){body{background-color:var(--color-gray-900)}}@media (prefers-color-scheme: dark){body{color:var(--color-gray-100)}}}@property --tw-translate-x{syntax:"*";inherits:false;initial-value:0;}@property --tw-translate-y{syntax:"*";inherits:false;initial-value:0;}@property --tw-translate-z{syntax:"*";inherits:false;initial-value:0;}@property --tw-scale-x{syntax:"*";inherits:false;initial-value:1;}@property --tw-scale-y{syntax:"*";inherits:false;initial-value:1;}@property --tw-scale-z{syntax:"*";inherits:false;initial-value:1;}@property --tw-rotate-x{syntax:"*";inherits:false;initial-value:rotateX(0);}@property --tw-rotate-y{syntax:"*";inherits:false;initial-value:rotateY(0);}@property --tw-rotate-z{syntax:"*";inherits:false;initial-value:rotateZ(0);}@property --tw-skew-x{syntax:"*";inherits:false;initial-value:skewX(0);}@property --tw-skew-y{syntax:"*";inherits:false;initial-value:skewY(0);}@property --tw-space-y-reverse{syntax:"*";inherits:false;initial-value:0;}@property --tw-space-x-reverse{syntax:"*";inherits:false;initial-value:0;}@property --tw-divide-y-reverse{syntax:"*";inherits:false;initial-value:0;}@property --tw-border-style{syntax:"*";inherits:false;initial-value:solid;}@property --tw-gradient-position{syntax:"*";inherits:false;}@property --tw-gradient-from{syntax:"<color>";inherits:false;initial-value:#0000;}@property --tw-gradient-via{syntax:"<color>";inherits:false;initial-value:#0000;}@property --tw-gradient-to{syntax:"<color>";inherits:false;initial-value:#0000;}@property --tw-gradient-stops{syntax:"*";inherits:false;}@property --tw-gradient-via-stops{syntax:"*";inherits:false;}@property --tw-gradient-from-position{syntax:"<length-percentage>";inherits:false;initial-value:0%;}@property --tw-gradient-via-position{syntax:"<length-percentage>";inherits:false;initial-value:50%;}@property --tw-gradient-to-position{syntax:"<length-percentage>";inherits:false;initial-value:100%;}@property --tw-leading{syntax:"*";inherits:false;}@property --tw-font-weight{syntax:"*";inherits:false;}@property --tw-tracking{syntax:"*";inherits:false;}@property --tw-shadow{syntax:"*";inherits:false;initial-value:0 0 #0000;}@property --tw-shadow-color{syntax:"*";inherits:false;}@property --tw-inset-shadow{syntax:"*";inherits:false;initial-value:0 0 #0000;}@property --tw-inset-shadow-color{syntax:"*";inherits:false;}@property --tw-ring-color{syntax:"*";inherits:false;}@property --tw-ring-shadow{syntax:"*";inherits:false;initial-value:0 0 #0000;}@property --tw-inset-ring-color{syntax:"*";inherits:false;}@property --tw-inset-ring-shadow{syntax:"*";inherits:false;initial-value:0 0 #0000;}@property --tw-ring-inset{syntax:"*";inherits:false;}@property --tw-ring-offset-width{syntax:"<length>";inherits:false;initial-value:0px;}@property --tw-ring-offset-color{syntax:"*";inherits:false;initial-value:#fff;}@property --tw-ring-offset-shadow{syntax:"*";inherits:false;initial-value:0 0 #0000;}@property --tw-blur{syntax:"*";inherits:false;}@property --tw-brightness{syntax:"*";inherits:false;}@property --tw-contrast{syntax:"*";inherits:false;}@property --tw-grayscale{syntax:"*";inherits:false;}@property --tw-hue-rotate{syntax:"*";inherits:false;}@property --tw-invert{syntax:"*";inherits:false;}@property --tw-opacity{syntax:"*";inherits:false;}@property --tw-saturate{syntax:"*";inherits:false;}@property --tw-sepia{syntax:"*";inherits:false;}@property --tw-drop-shadow{syntax:"*";inherits:false;}@property --tw-backdrop-blur{syntax:"*";inherits:false;}@property --tw-backdrop-brightness{syntax:"*";inherits:false;}@property --tw-backdrop-contrast{syntax:"*";inherits:false;}@property --tw-backdrop-grayscale{syntax:"*";inherits:false;}@property --tw-backdrop-hue-rotate{syntax:"*";inherits:false;}@property --tw-backdrop-invert{syntax:"*";inherits:false;}@property --tw-backdrop-opacity{syntax:"*";inherits:false;}@property --tw-backdrop-saturate{syntax:"*";inherits:false;}@property --tw-backdrop-sepia{syntax:"*";inherits:false;}@property --tw-duration{syntax:"*";inherits:false;}@property --tw-ease{syntax:"*";inherits:false;}@property --tw-outline-style{syntax:"*";inherits:false;initial-value:solid;}</style><link rel="stylesheet" href="styles-WSREB2OI.css" media="print" onload="this.media='all'"><noscript><link rel="stylesheet" href="styles-WSREB2OI.css"></noscript></head>
<body ngcm="">
  <app-root></app-root>
<link rel="modulepreload" href="chunk-RCTVI3O5.js"><link rel="modulepreload" href="chunk-T3LIHWEP.js"><link rel="modulepreload" href="chunk-4YLKNM5S.js"><link rel="modulepreload" href="chunk-GJDY2YIJ.js"><link rel="modulepreload" href="chunk-ZGC3T5ZG.js"><link rel="modulepreload" href="chunk-FWRIDW2Q.js"><script src="polyfills-WEMCER7C.js" type="module"></script><script src="main-K4TLIQBV.js" type="module"></script></body>
</html>
`;