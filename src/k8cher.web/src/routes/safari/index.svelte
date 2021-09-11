<script>
    import Page from '$lib/page/Page.svelte'
    import Button from '$lib/Button.svelte'
    import { safariStore } from '$lib/safariStore'
    import AnimalControl from '$lib/safari/AnimalControl.svelte'

    
    let newAnimalName;

    function handleKeyUp(event) {
        if (event.key !== "Enter" || newAnimalName.length === 0) {
            return;
        }
        safariStore.addAnimal(newAnimalName)
        newAnimalName = ''
    }
</script>

<svelte:head>
	<title>Safari Search</title>
</svelte:head>
<Page>
    <AnimalControl />
    <!-- <Button on:click="{init}">test initialize store</Button> -->
    <input type="text"  placeholder="What animal do you see?" bind:value={newAnimalName} on:keyup={handleKeyUp} >

    {#each $safariStore.animals as animal}
        <h1>{animal.name}</h1>
    {/each}
</Page>