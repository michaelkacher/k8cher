import { writable } from 'svelte/store'
import { persistable } from './persistable';


const initialState = {
    name: '',
    date: undefined,
    animals: [],
}

export const safariStore = createSafariStore();

function createSafariStore() {
    // const { subscribe, update } = writable(initialState)
    const { subscribe, update } = persistable('safari', initialState)

    return {
        subscribe,
        addAnimal: async (animalName) => {
                update(state => {
                    return { ...state, animals: [...state.animals, {name: animalName} ] }
                })
        },
        setName: async(name) => {
            update(state => (state = { ...state, name }));
        },
        setLocation: async(date) => {
            update(state => (state = { ...state, date }));
        }
    };
}

