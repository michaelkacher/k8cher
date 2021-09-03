<script>
	import { userStore } from '$lib/auth/userStore'

	export let role = 'user'
	export let showAccessDeniedMessage = false

	function hasRole() {
		return (
			$userStore.roles &&
			$userStore.roles.length &&
			$userStore.roles.includes(role)
		)
	}
</script>

<div>
	{#if hasRole()}
		<slot />
	{:else if showAccessDeniedMessage}
		<div class="text-red-300 text-xl text-center mt-4">
			User is not authorized to access the page
		</div>
	{/if}
</div>
