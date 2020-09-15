
var civicCookieControlConfig = {
  apiKey: 'fc8364648ad258321fa6197d91d72771562de378',
  product: 'PRO',
  logConsent: true,
  initialState: 'notify',
  notifyDismissButton: false,
  rejectButton: false,
  setInnerHTML: true,
  closeStyle: 'button',
  text: {
    notifyTitle: '<span class="civic-cookie-banner-title">We use cookies</span>',
    notifyDescription: 'Some of these cookies are essential, while others help us to improve your experience by providing insights into how the site is being used.'
      + ' For more detailed information, please read our <a href="/cookies" class="civic-cookie-control-link">cookie notice</a>.',
    accept: 'Accept all cookies',
    reject: 'Customise settings',
    title: 'We use cookies',
    settings: 'Customise settings',
    intro: '<p>Some of these cookies are essential, while others help us to improve your experience by providing insights into how the site is being used.</p>'
      + '<p>For more detailed information, please read our <a href="../about-jncc/corporate-information/cookie-policy/" class="civic-cookie-control-link">cookie notice</a>.</p>',
    necessaryTitle: 'Essential cookies',
    necessaryDescription: 'Essential cookies enable core functionality. The site cannot function without these cookies and can only be disabled by changing your browser preferences.',
    closeLabel: 'Save settings'  // closing doesn't actually *set* anything - settings already saved on toggle!
  },
  branding: {
    removeAbout: true,
    backgroundColor: '#222',
    toggleColor: '#0065bd',
    fontFamily: 'Hind Madurai, "Helvetica Neue", Helvetica, Arial, sans-serif',
    fontSize: '1em',
    fontSizeTitle: '2em'
  },
  optionalCookies: [
    {
        name : 'analytics',
        recommendedState: false,
        label: 'Analytics cookies',
        description: 'Please let us set analytical cookies to help us improve our website by collecting and reporting information about how you use it, in a way that does not directly identify anyone.',
        cookies: ['_gat_UA-15841534-6', '_gid', '_ga'],
        onAccept : function() {

            dataLayer.push({'event': 'analytics_consent_accepted'});
        },
        onRevoke: function() {

            dataLayer.push({'event': 'analytics_consent_withdrawn'});
            window['ga-disable-UA-15841534-6'] = true;

        }
    }
  ]
};

CookieControl.load(civicCookieControlConfig);
