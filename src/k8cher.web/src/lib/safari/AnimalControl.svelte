<script>
	import { safariStore } from '$lib/safariStore'
	import Button from '$lib/Button.svelte'	
	
	function addMonkey() {
		safariStore.addAnimal('Monkey')
	}

	function addSnake() {
		safariStore.addAnimal('Snake')
	}

	function addSloth() {
		safariStore.addAnimal('Sloth')
	}
</script>

<Button on:click="{addMonkey}">Add Monkey</Button>
<Button on:click="{addSnake}">Add Snake</Button>
<Button on:click="{addSloth}">Add Sloth</Button>