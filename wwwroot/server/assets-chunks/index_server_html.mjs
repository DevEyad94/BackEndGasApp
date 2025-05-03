export default `<!doctype html>
<html lang="en">
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
<link rel="stylesheet" href="styles-CWCGAFOA.css"></head>
<body><script type="text/javascript" id="ng-event-dispatch-contract">(()=>{function p(t,n,r,o,e,i,f,m){return{eventType:t,event:n,targetElement:r,eic:o,timeStamp:e,eia:i,eirp:f,eiack:m}}function u(t){let n=[],r=e=>{n.push(e)};return{c:t,q:n,et:[],etc:[],d:r,h:e=>{r(p(e.type,e,e.target,t,Date.now()))}}}function s(t,n,r){for(let o=0;o<n.length;o++){let e=n[o];(r?t.etc:t.et).push(e),t.c.addEventListener(e,t.h,r)}}function c(t,n,r,o,e=window){let i=u(t);e._ejsas||(e._ejsas={}),e._ejsas[n]=i,s(i,r),s(i,o,!0)}window.__jsaction_bootstrap=c;})();
</script>
  <app-root></app-root>
<link rel="modulepreload" href="chunk-YUDK37QP.js"><link rel="modulepreload" href="chunk-X5ZEARGI.js"><link rel="modulepreload" href="chunk-VYCE342H.js"><script src="polyfills-WEMCER7C.js" type="module"></script><script src="main-LGWPK6Z7.js" type="module"></script></body>
</html>
`;