<script>
    import Page from "$lib/page/Page.svelte";
    import Button from "$lib/Button.svelte";
    import { createListStore } from '$lib/listStore'
    import { goto } from "$app/navigation";
    import Notes from "$lib/notes/Notes.svelte"
    import {v4 as uuidv4} from 'uuid'
    import { onMount } from 'svelte'

    let pickupDate = new Date()
    let orderList

    onMount(() => {
		orderList = createListStore('customerDashboard')
	})

    function addOrder() {
        orderList.addItem({
            id: uuidv4(),
            store: "My store",
            pickupDate,
            status: "In Cart",
        })
    }

    function viewDetails(id) {
        goto(`/orders/order/${id}`);
    }
</script>

<svelte:head>
    <title>My Orders</title>
</svelte:head>
<Page>
    {#if $orderList && $orderList.items}
    <h1>My orders</h1>
    <!-- select store -->
    <input type="date" bind:value={pickupDate} />
    <Button on:click={addOrder}>Add order</Button>

    {#each $orderList.items as order (order.id)}
        <h1>Pickup Store: {order.store}</h1>
        <h1>Id: {order.id}</h1>
        <h2>Pickup Date: {order.pickupDate}</h2>
        <h3>Status: {order.status}</h3>
        <Notes nodeId="{order.id}" />
    {/each}
    {/if}
</Page>
