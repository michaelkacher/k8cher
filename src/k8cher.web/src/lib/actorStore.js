import { writable } from 'svelte/store'
import { serverUrl } from '$lib/utils/env'
import { post, get } from '$lib/utils/apiHelper'

const initialState = {
    actorState: undefined,
    actions: [],
    isLoading: false,
    error: ""
}

export const actorStore = createActorStore('blarg')

function createActorStore(storeName) {
    const { subscribe, set, update } = writable(initialState)

    return {
        subscribe,
        initializeFromApi: async () => {
            update(state => (state = { ...state, isLoading: true }))
            try {
                const res = await get(`${serverUrl}store/get?storeName=${storeName}`)
                if (res.success) {
                    update(state => (state = { ...state, actorState: res.json }))
                    // set(JSON.parse(res.json))
                }

            } catch (e) {
                // todo - mbk: add to error service toast
            } finally {
                update(state => (state = { ...state, isLoading: false }))
            }
        },
        // a property to update and the json value to be updated
        set: async (json) => {
            update(state => (state = { ...state, isLoading: true }))
            const res = await post(`${serverUrl}store/set`, {
                name: storeName,
                json
            })

            if (res.success) {
                // todo - mbk: need to determine how best to set, right now doing
                // update so store can have is loading. But if change to eager,
                // just have value of store?
                // Also should this be set eagerly?
                update(state => (state = { ...state, actorState: json }))
                //set(json)
            }

            update(state => (state = { ...state, isLoading: false }))
        },
        // todo - mbk: not complete, this is if want to update individual properties
        // updateProperty: async (propertyName, json) => {
        //     update(state => (state = { ...state, isLoading: true }));
        //     const res = await post(`${serverUrl}store/update`, {
        //     })

        //     if (res.success) {
        //         update(state => (state = { ...state, actorState: json }));
        //     }
        //     update(state => (state = { ...state, isLoading: false }));
        // }
    };
}

