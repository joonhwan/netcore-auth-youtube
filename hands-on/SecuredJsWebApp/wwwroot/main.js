const config = {
    userStore: new Oidc.WebStorageStateStore({store: window.localStorage}),
    authority: "https://localhost:50001",
    client_id: "mirero.secured.web.app",
    redirect_uri: "https://localhost:60011/signin",
    // post_logout_redirect_uri: "https://localhost:44345/Home/Index",
    // response_type: "code",
    response_type: "id_token token",
    scope: "openid scope.mirero.profile scope.mirero.api.type.secret scope.mirero.api.type.gateway"
};

const userManager = new Oidc.UserManager(config);

const signIn = function () {
    userManager.signinRedirect(); // 로그인이 안되어 있으면, 인증서버쪽으로 페이지 이동이 일어난다. 
};

// const signOut = function () {
//     userManager.signoutRedirect();
// };

// 로그아웃 된 상태이면, Sign In 버튼을 표시하는데 사용.
const showSignin = visible => {
    document.getElementById("signin").style.display = visible ? 'block' : 'none';
}

userManager.getUser().then(user => {
    //console.log("user:", user);
    console.log("found active user : ", user.profile.name)
    if (user) {
        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
    } else {
        showSignin(true);
    }
}).catch(e => {
    console.log("not logged in state!")
    console.log(e);
    showSignin(true);
});

const callApi = function () {
    axios.get("https://localhost:51001/api/v1/secret")
        .then(res => {
            console.log(res);
        });
};

let refreshing = false;

axios.interceptors.response.use(
    // 성공한 경우,  
    function (response) { return response; },
    // 실패한 경우,
    function (error) {
        //console.log("axios error:", error);
        console.log("axios error.response:", error.response);

        const axiosConfig = error.response.config; // request 시 사용한 설정 내역.

        //만일 401(Unauthorized) 이면, refresh_token 을 수행.
        if (error.response.status === 401) {
            console.log("axios error 401");

            // if already refreshing don't make another request
            if (!refreshing) {
                console.log("starting token refresh");
                refreshing = true;

                // do the refresh
                return userManager
                    .signinSilent()
                    .then(user => {
                        // 새로 발급된 access_token...등의 유효 기간 확인
                        const expire_at = new Date(user.expires_at * 1000)
                        console.log("new access_token expires at ", expire_at.toISOString());

                        // 이후, axios 를 통한 모든 request 에서 쓸 공통 헤더를 갱신
                        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;

                        // 현 request 를 새로운 access_token 으로 다시 시도.
                        axiosConfig.headers["Authorization"] = "Bearer " + user.access_token;
                        return axios(axiosConfig);
                    })
                    .catch(() => {
                        console.error("refresh token 실패")
                    })
                    .finally(() => {
                        // refreshing 종료 
                        refreshing = false;
                    })
                ;
                
            }
        }

        return Promise.reject(error);
    });