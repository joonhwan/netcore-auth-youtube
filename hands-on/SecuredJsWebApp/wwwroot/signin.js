const userManager = new Oidc.UserManager();

userManager.signinCallback().then(res => {
    console.log(res);
    window.location.href = "/";
})
