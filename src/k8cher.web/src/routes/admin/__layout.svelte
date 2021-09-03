<script>
	import RoleGuard from '$lib/auth/RoleGuard.svelte'
</script>

<RoleGuard role="admin" showAccessDeniedMessage="true">
	<slot />
</RoleGuard>