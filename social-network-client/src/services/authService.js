import { Post } from './httpService';
import { setJwt } from './jwtService';
import { connect } from './notificationsService';
const authUrl = process.env.REACT_APP_AUTH_URL;
export async function register(data) {
    const res = await Post(authUrl + 'register', data);

    if (res.status === 400) return await res.json();
    else if (res.status !== 200) return 'something faild';

    const jwt = res.headers.get('x-auth-token');
    if (jwt) {
        setJwt(jwt);
        connect();
    }
}

export async function login(data) {
    const res = await Post(authUrl + 'login', data);

    if (res.status === 400) return await res.json();
    else if (res.status !== 200) return 'something faild';

    const jwt = res.headers.get('x-auth-token');
    if (jwt) {
        setJwt(jwt);
        connect();
    }
}

export async function facebookLogin(facebookToken) {
    const res = await Post(
        authUrl + 'loginFacebook',
        JSON.stringify(facebookToken.accessToken)
    );

    if (res.status === 400) return await res.json();
    else if (res.status !== 200) return 'something faild';

    const jwt = res.headers.get('x-auth-token');
    setJwt(jwt);
}

export async function resetPassword(data) {
    return await Post(authUrl + 'resetPassword', data);
}
