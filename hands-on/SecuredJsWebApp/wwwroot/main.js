const config = {
    // userStore: new Oidc.WebStorageStateStore({store: window.localStorage}),
    authority: "https://localhost:50001",
    client_id: "mirero.secured.web.app",
    redirect_uri: "https://localhost:60011/signin",
    // post_logout_redirect_uri: "https://localhost:44345/Home/Index",
    // response_type: "code",
    response_type: "id_token token",
    scope: "openid scope.mirero.profile scope.mirero.api.type.secret"
};

const userManager = new Oidc.UserManager(config);

const signIn = function () {
    userManager.signinRedirect();
};

// const signOut = function () {
//     userManager.signoutRedirect();
// };

userManager.getUser().then(user => {
    console.log("user:", user);
    if (user) {
        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
    }
});

const callApi = function () {
    axios.get("https://localhost:51001/api/v1/secret")
        .then(res => {
            console.log(res);
        });
};

let refreshing = false;

// axios.interceptors.response.use(
//     function (response) { return response; },
//     function (error) {
//         console.log("axios error:", error.response);
//
//         const axiosConfig = error.response.config;
//
//         //if error response is 401 try to refresh token
//         if (error.response.status === 401) {
//             console.log("axios error 401");
//
//             // if already refreshing don't make another request
//             if (!refreshing) {
//                 console.log("starting token refresh");
//                 refreshing = true;
//
//                 // do the refresh
//                 return userManager.signinSilent().then(user => {
//                     console.log("new user:", user);
//                     //update the http request and client
//                     axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
//                     axiosConfig.headers["Authorization"] = "Bearer " + user.access_token;
//                     //retry the http request
//                     return axios(axiosConfig);
//                 });
//             }
//         }
//
//         return Promise.reject(error);
//     });