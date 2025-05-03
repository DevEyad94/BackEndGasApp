
export default {
  bootstrap: () => import('./main.server.mjs').then(m => m.default),
  inlineCriticalCss: true,
  baseHref: '/',
  locale: undefined,
  routes: [
  {
    "renderMode": 2,
    "route": "/"
  },
  {
    "renderMode": 1,
    "route": "/search/*"
  },
  {
    "renderMode": 2,
    "route": "/story"
  },
  {
    "renderMode": 1,
    "route": "/ar-view"
  },
  {
    "renderMode": 2,
    "preload": [
      "chunk-ULQJZWJQ.js"
    ],
    "route": "/login"
  },
  {
    "renderMode": 2,
    "preload": [
      "chunk-ULQJZWJQ.js"
    ],
    "route": "/admin/login"
  },
  {
    "renderMode": 2,
    "preload": [
      "chunk-6WEHRBKK.js"
    ],
    "route": "/grave/management"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-4FKRRPKJ.js"
    ],
    "route": "/grave/management/new"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-4FKRRPKJ.js"
    ],
    "route": "/grave/edit/*"
  },
  {
    "renderMode": 1,
    "route": "/grave/*"
  },
  {
    "renderMode": 2,
    "preload": [
      "chunk-OQQXUETP.js"
    ],
    "route": "/unauthorized"
  },
  {
    "renderMode": 2,
    "redirectTo": "/",
    "route": "/**"
  }
],
  entryPointToBrowserMapping: undefined,
  assets: {
    'index.csr.html': {size: 13892, hash: 'e19bdc46c47565c7ebab575397960956053474c3ee1be74ebe785e14d4223257', text: () => import('./assets-chunks/index_csr_html.mjs').then(m => m.default)},
    'index.server.html': {size: 5222, hash: '018c0f6c5ac75d8b11c3a33bdbe51fa5dd29e8e8fd816b465aef759f15a37671', text: () => import('./assets-chunks/index_server_html.mjs').then(m => m.default)},
    'story/index.html': {size: 65794, hash: '64d82425c33b7b3c665ca2270a8aacc97e49d96e9d454f3919bb701ccdda504c', text: () => import('./assets-chunks/story_index_html.mjs').then(m => m.default)},
    'login/index.html': {size: 61955, hash: '06c3ad403c6982b8a1fdbbfc9297091a862db7573df9b8390d4e61d872acae85', text: () => import('./assets-chunks/login_index_html.mjs').then(m => m.default)},
    'admin/login/index.html': {size: 61955, hash: '06c3ad403c6982b8a1fdbbfc9297091a862db7573df9b8390d4e61d872acae85', text: () => import('./assets-chunks/admin_login_index_html.mjs').then(m => m.default)},
    'grave/management/index.html': {size: 61955, hash: 'd651c52c5ab861b296fd8254e2fd54eec80a20deb0c958a97abf49613653d203', text: () => import('./assets-chunks/grave_management_index_html.mjs').then(m => m.default)},
    'unauthorized/index.html': {size: 56593, hash: 'e6f9237a3ccaf11d7038faad2f60c1e0e5012b2ebc833de7f701b1e17f8bd37a', text: () => import('./assets-chunks/unauthorized_index_html.mjs').then(m => m.default)},
    'index.html': {size: 58546, hash: '118cfe7510e7ab7a3cf16aa23df3ebdfa74b0576744646a3fca07e15e5c3b5fd', text: () => import('./assets-chunks/index_html.mjs').then(m => m.default)},
    'styles-WSREB2OI.css': {size: 53331, hash: 'Tx/Oh6gG52o', text: () => import('./assets-chunks/styles-WSREB2OI_css.mjs').then(m => m.default)}
  },
};
