
export default {
  bootstrap: () => import('./main.server.mjs').then(m => m.default),
  inlineCriticalCss: true,
  baseHref: '/',
  locale: undefined,
  routes: [
  {
    "renderMode": 1,
    "preload": [
      "chunk-VCMKRQTY.js",
      "chunk-HUA2HUQ3.js"
    ],
    "route": "/"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-55C6AUOO.js",
      "chunk-HFMPSTJO.js",
      "chunk-JVRJCTEF.js",
      "chunk-HUA2HUQ3.js"
    ],
    "route": "/dashboard"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-TTBHDDQB.js",
      "chunk-HFMPSTJO.js",
      "chunk-JVRJCTEF.js",
      "chunk-HUA2HUQ3.js"
    ],
    "route": "/map"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-W3RO432Q.js",
      "chunk-LF3X5EDQ.js",
      "chunk-JVRJCTEF.js",
      "chunk-HUA2HUQ3.js"
    ],
    "route": "/production"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-EZ4P43AE.js",
      "chunk-LF3X5EDQ.js",
      "chunk-JVRJCTEF.js",
      "chunk-HUA2HUQ3.js"
    ],
    "route": "/maintenance"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-KQMIA4KT.js"
    ],
    "route": "/colors"
  },
  {
    "renderMode": 1,
    "preload": [
      "chunk-NZFKQFRB.js"
    ],
    "route": "/unauthorized"
  },
  {
    "renderMode": 1,
    "redirectTo": "/dashboard",
    "route": "/**"
  }
],
  entryPointToBrowserMapping: undefined,
  assets: {
    'index.csr.html': {size: 9800, hash: '785c7328bacef79bdb0ddb5a1b4e4ebf879c7357d4e2c3ccfc868fd2bb96eacc', text: () => import('./assets-chunks/index_csr_html.mjs').then(m => m.default)},
    'index.server.html': {size: 1875, hash: 'e55036456fbb088218cbed6c16267a9f51d4a996847d0b34db8a315a62c9629a', text: () => import('./assets-chunks/index_server_html.mjs').then(m => m.default)},
    'styles-VUL4RJLQ.css': {size: 49157, hash: 'xuZqSdKFQBk', text: () => import('./assets-chunks/styles-VUL4RJLQ_css.mjs').then(m => m.default)}
  },
};
