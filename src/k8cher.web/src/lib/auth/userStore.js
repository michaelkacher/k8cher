import { writable } from 'svelte/store'
import { browser } from '$app/env'

// todo - mbk: move into a util
function parseJwt(token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    // todo - mbk: is atob deprecated in this case, research and refactor
    var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
}

const initialState = {
    jwt: undefined,
    user: undefined,
    actions: [],
    roles: [],
    isLoading: false,
    successfulLoad: false,
    error: ""
}

export const userStore = createUserStore();

function createUserStore() {
    const { subscribe, set, update } = writable(initialState, (setFunc) => {
        update(state => (state = { ...state, isLoading: true }));
        try {
            if (browser) {
                let jwt = sessionStorage.getItem('jwt')
                if (jwt == null) {
                    jwt = localStorage.getItem('jwt')
                }
                if (jwt) {

                    const jsonJwt = parseJwt(jwt)
                    if (jsonJwt['UserRole']) {
                        const myProp = 'roles';
                        const updateValue = {}
                        updateValue[myProp] = jsonJwt['UserRole']
                        update(state => (state = { ...state, ...updateValue }));
                    }
                    update(state => (state = { ...state, jwt: jwt }));
                    // todo - mbk: previously only returned e-mail user with e-mail, determine next steps 
                    const json = { email: 'test@test.com' }
                    update(state => (state = { ...state, user: json }));
                    update(state => (state = { ...state, successfulLoad: true }));
                }
            }
        } catch (e) {
            console.log('error initializing: ' + e)
            // todo - mbk: add to error service toast
        } finally {
            update(state => (state = { ...state, isLoading: false }));
        }

        return () => console.log(`no more subscribers for user store`)
    })

    return {
        subscribe,
        loadFromJwt: async (jwt, rememberMe) => {
            update(state => (state = { ...state, isLoading: true }));
            try {
                if (rememberMe) {
                    localStorage.setItem('jwt', jwt)
                } else {
                    sessionStorage.setItem('jwt', jwt)
                }

                const jsonJwt = parseJwt(jwt)
                if (jsonJwt['UserRole']) {
                    update(state => (state = { ...state, roles: jsonJwt['UserRole'] }));
                }

                update(state => (state = { ...state, jwt: jwt }));

                // todo - mbk: previously only returned e-mail user with e-mail, determine next steps 
                // const res = await get(`${serverUrl}auth/me`)
                const json = { email: 'test@test.com' }
                update(state => (state = { ...state, user: json }));

                update(state => (state = { ...state, successfulLoad: true }));
            } catch (e) {
                // todo - mbk: add to error service toast
            } finally {
                update(state => (state = { ...state, isLoading: false }));
            }
        },
        logout: async () => {
            update(state => (state = { ...state, isLoading: true }));
            try {
                if (browser) {
                    localStorage.removeItem('jwt');
                    sessionStorage.removeItem('jwt');
                }
                set(initialState)
            } catch (e) {
                // todo - mbk: add to error service toast
            } finally {
                update(state => (state = { ...state, isLoading: false }));
            }
        }
    };
}