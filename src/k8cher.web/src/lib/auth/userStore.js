import { writable } from 'svelte/store'
import { get } from '$lib/utils/apiHelper'
import { browser } from '$app/env'
import { serverUrl } from '$lib/utils/env'

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
    const { subscribe, set, update } = writable(initialState);

    return {
        subscribe,
        loadFromJwt: async (jwt, rememberMe) => {
            update(state => (state = { ...state, isLoading: true }));
            try {
                // console.log('browser: ' + browser)
                // if (browser) {
                if (rememberMe) {
                    localStorage.setItem('jwt', jwt)
                } else {
                    sessionStorage.setItem('jwt', jwt)
                }
                // }

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
                alert(e.message); // todo - mbk: make part of apiHelper and remove alert
            } finally {
                update(state => (state = { ...state, isLoading: false }));
            }
        },
        initialize: async () => {
            console.log('start')
            update(state => (state = { ...state, isLoading: true }));
            console.log('before try')
            try {
                console.log('browser = ' + browser)

                if (browser) {
                    console.log('attempting session storage getzz')
                    let jwt = sessionStorage.getItem('jwt')
                    console.log('jwt is: ' + jwt)
                    if (jwt == null) {
                        console.log('attempting local storage get')
                        jwt = localStorage.getItem('jwt')
                    }
                    console.log('continuing.......')
                    console.log('!!!!!!!jwt is: ' + jwt)
                    if (jwt) {

                        const jsonJwt = parseJwt(jwt)
                        if (jsonJwt['UserRole']) {
                            const myProp = 'roles';
                            const updateValue = {}
                            updateValue[myProp] = jsonJwt['UserRole']
                            update(state => (state = { ...state, ...updateValue }));


                            // update(state => (state = { ...state, roles: jsonJwt['UserRole'] }));
                        }

                        update(state => (state = { ...state, jwt: jwt }));
                        console.log('updating jwt complete')
                        // todo - mbk: previously only returned e-mail user with e-mail, determine next steps 
                        // const res = await get(`${serverUrl}auth/me`)
                        const json = { email: 'test@test.com' }
                        update(state => (state = { ...state, user: json }));
                        console.log('updating user complete')
                        update(state => (state = { ...state, successfulLoad: true }));
                    }
                }
            } catch (e) {
                console.log("error: " + e)
                alert(e.message); // todo - mbk: make part of apiHelper and remove alert
            } finally {
                console.log('is loadding to false')
                update(state => (state = { ...state, isLoading: false }));
            }
        },
        logout: async () => {
            update(state => (state = { ...state, isLoading: true }));
            try {
                // if (browser) {
                localStorage.removeItem('jwt');
                sessionStorage.removeItem('jwt');
                // }
                // todo - mkb: test if resetting initial state here works instead 
                // of updating each individually
                update(state => (state = { ...state, jwt: undefined }));
                update(state => (state = { ...state, user: undefined }));
                update(state => (state = { ...state, successfulLoad: false }));

                // todo - mbk: should a future post be added to add to invalidated list?
            } catch (e) {
                alert(e.message); // todo - mbk: make part of apiHelper and remove alert
            } finally {
                update(state => (state = { ...state, isLoading: false }));
            }
        }
    };
}