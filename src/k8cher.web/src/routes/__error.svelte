<script>
	export let status
	export let error
</script>

<svelte:head>
	<title>{status}</title>
</svelte:head>
<p>Unexpected error</p>
<h1>{status}</h1>

<p>{error}</p>
