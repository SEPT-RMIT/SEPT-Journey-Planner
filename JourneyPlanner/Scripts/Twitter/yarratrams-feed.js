new TWTR.Widget({
    version: 2,
    type: 'profile',
    rpp: 4,
    interval: 30000,
    width: 250,
    height: 500,
    theme: {
        shell: {
            background: '#333333',
            color: '#ffffff'
        },
        tweets: {
            background: '#EFEEEF',
            color: '#333333',
            links: '#7AC0DA'
        }
    },
    features: {
        scrollbar: false,
        loop: false,
        live: false,
        behavior: 'all'
    }
}).render().setUser('yarratrams').start();