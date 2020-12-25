const userStore = new Oidc.WebStorageStateStore({store: window.localStorage})
const userManager = new Oidc.UserManager({
    userStore,
});

function delay(t, v) {
    return new Promise(function(resolve) {
        setTimeout(resolve.bind(null, v), t)
    });
}
Promise.prototype.delay = function(t) {
    return this.then(function(v) {
        return delay(t, v);
    });
}

userManager
    .signinCallback()
    .then(res => {
        console.log("signin callback is called ! res=", res)
        window.location.href = "/"; // SPA App 에서는 "/../#PageName" 같이 될 거 같음.
    })
;
