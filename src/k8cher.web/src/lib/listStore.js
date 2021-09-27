import { writable } from 'svelte/store'
// import { persistable } from './persistable'
import { localPersistable } from  './localPersistable'

const initialState = {
    items: [],
}

export function getListStore() {

}

export function createListStore(id) {
    // todo - mbk: have this set at env level?
    // const { subscribe, update } = writable(initialState)
    // const { subscribe, update } = persistable('orderDashboard', initialState)
    const { subscribe, update } = localPersistable(id, initialState)

    return {
        subscribe,
        // todo - mbk: remove the async
        addItem: async (item) => {
                update(state => {
                    return { ...state, items: [...state.items, item ] }
                })
        },
        deleteItem: async (item) => {
            update(state => {
                return { ...state, items: state.items.filter(i => i.id !== item.id) }
            })
        },
        updateItem: async (item) => {
            update(state => {
                return { ...state, items: state.items.map(i => {
                    if (i.id == item.id) {
                        return item
                    }
                    
                    return i;
                }) }
            })
        }
    };
}
