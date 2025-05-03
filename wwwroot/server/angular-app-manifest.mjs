
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
    "renderMode": 2,
    "preload": [
      "chunk-TEI5ASX3.js"
    ],
    "route": "/colors"
  },
  {
    "renderMode": 2,
    "preload": [
      "chunk-NYALMEPK.js"
    ],
    "route": "/login"
  },
  {
    "renderMode": 2,
    "preload": [
      "chunk-NYALMEPK.js"
    ],
    "route": "/admin/login"
  },
  {
    "renderMode": 2,
    "preload": [
      "chunk-SNSRYH6V.js"
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
    'index.csr.html': {size: 12070, hash: '60077d4eff0481646f92d24ccef64f40f58cceb64f726ff48b5579792ef6475d', text: () => import('./assets-chunks/index_csr_html.mjs').then(m => m.default)},
    'index.server.html': {size: 5069, hash: '74581a9edfe928e81128e69ddc9336e3ac943fdaf527621b2fb24435e889160a', text: () => import('./assets-chunks/index_server_html.mjs').then(m => m.default)},
    'admin/login/index.html': {size: 56636, hash: '9b802133daa4ed836d51a74e5b339e0aee8f323b9b3495f47751691a7d8ab743', text: () => import('./assets-chunks/admin_login_index_html.mjs').then(m => m.default)},
    'index.html': {size: 56769, hash: '2d3e6f365b8612f9548cefd4366821859c9ecc269de6ee6c21de591c75336b87', text: () => import('./assets-chunks/index_html.mjs').then(m => m.default)},
    'login/index.html': {size: 56636, hash: '9b802133daa4ed836d51a74e5b339e0aee8f323b9b3495f47751691a7d8ab743', text: () => import('./assets-chunks/login_index_html.mjs').then(m => m.default)},
    'colors/index.html': {size: 58761, hash: '074ebaad4eb8e891995d12c3b871bdf91ffaa21e90da97a8f2392c52c8594ab3', text: () => import('./assets-chunks/colors_index_html.mjs').then(m => m.default)},
    'unauthorized/index.html': {size: 50718, hash: '15a7d2ca334019e890ab6b8865d8603f9a338a41e3ec282a15478b3d8c065ebb', text: () => import('./assets-chunks/unauthorized_index_html.mjs').then(m => m.default)},
    'styles-CWCGAFOA.css': {size: 40814, hash: 'fvdkDbBA6Cs', text: () => import('./assets-chunks/styles-CWCGAFOA_css.mjs').then(m => m.default)}
  },
};
