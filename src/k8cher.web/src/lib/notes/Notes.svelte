<script>
	import { createListStore } from '$lib/listStore'
	import Button from "$lib/Button.svelte";
	import { onMount } from 'svelte'
	import {v4 as uuidv4} from 'uuid'

	export let nodeId = 'test-id'
	let myListStore

	onMount(() => {
		console.log(nodeId)
		myListStore = createListStore(nodeId)

	})

	function addItem() {
        myListStore.addItem({
            id: uuidv4(),
            store: "My store",
            status: "In Cart",
			count: 1,
        });
    }	
	
	function deleteItem(item) {
		myListStore.deleteItem(item)
	}

	function increaseCount(item) {
		item.count = item.count + 1;
		myListStore.updateItem(item)
	}

	function decreaseCount(item) {
		item.count = item.count - 1;
		if (item.count <= 0) {
			deleteItem(item)
		}
		else {
			myListStore.updateItem(item)
		}
	}

</script>

<div>
	<h1>My items</h1>
	{#if $myListStore && $myListStore.items}
	
    <!-- select store -->
    <Button on:click={addItem}>Add item</Button>

    {#each $myListStore.items as item}
        <h1>Pickup Store: {item.store}</h1>
        <h3>Status: {item.status}</h3>
		<h3>Count: {item.count}</h3>
		<Button on:click={increaseCount(item)}>+</Button>
		<Button on:click={decreaseCount(item)}>-</Button>
		<Button on:click={deleteItem(item)}>Delete</Button>
    {/each}

	{/if}
</div>
