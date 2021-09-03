<script>
	import { page } from '$app/stores'

	import AuthGuard from '$lib/auth/AuthGuard.svelte'
	import Footer from '$lib/layout/Footer.svelte'
	import Navbar from '$lib/layout/Navbar.svelte'
	import Notification from '$lib/notification/Notification.svelte'
	import { userStore } from '$lib/auth/userStore'
	import { onMount } from 'svelte'
	import '../global.css'
	import '../tailwind.css'

	// todo - mbk: don't like this solution, would prefer a __layout.reset.svelte
	$: isLoginRouteActivated = $page && $page.path && $page.path === '/login'

	onMount(async () => {
		await userStore.initialize()
	});
</script>

<!-- {#await userStore.initialize() then response} -->
	<div
		class="flex flex-col text-gray-900 dark:text-gray-100 bg-gray-100 dark:bg-gray-900 antialiased min-h-screen font-sans"
	>
		<div class="z-10" class:hidden="{isLoginRouteActivated}">
			<Navbar />
		</div>
		<Notification />
		<main class="flex-grow">
			<AuthGuard>
				<slot />
			</AuthGuard>
		</main>
		<div class:hidden="{isLoginRouteActivated}">
			<Footer />
		</div>
	</div>
<!-- {/await} -->
