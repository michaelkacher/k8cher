<script>
	import { goto } from '$app/navigation'
	import { page } from '$app/stores'
	import { onMount } from 'svelte'
	import { userStore } from '$lib/auth/userStore.js'

	const allowedUnAuthenticatedRoutes = [
		'/login',
		'/account/register',
		'/account/activate',
		'/account/reset/init',
	]

	$: routeAccessAllowed =
		($userStore.successfulLoad) ||
		($page &&
			$page.path &&
			[...allowedUnAuthenticatedRoutes, '/'].includes($page.path))

	function checkIfCurrentRouteAccessNotAllowed() {
		if (
			$userStore.successfulLoad &&
			$page &&
			$page.path &&
			allowedUnAuthenticatedRoutes.includes($page.path)
		) {
			goto('/')
		} else if (!routeAccessAllowed) {
			goto('/login')
		}
	}
	onMount(() => checkIfCurrentRouteAccessNotAllowed())
</script>

{#if routeAccessAllowed}
	<slot />
{/if}
