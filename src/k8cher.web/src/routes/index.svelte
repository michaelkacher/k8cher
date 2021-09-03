<script>
	import { userStore } from '$lib/auth/userStore'
	import { actorStore } from '$lib/actorStore'
	import Button from '$lib/Button.svelte'

	async function setStore() {
		await actorStore.set({
			test: 'rawr',
			hello: 'world!'
		})
	}
</script>

<svelte:head>
	<title>K8cher Web</title>
	<meta name="Description" content="Move fast!" />
</svelte:head>

<section
	class="m-3 bg-white dark:bg-gray-800 shadow-lg rounded p-4 border border-gray-200 dark:border-gray-700 transition-colors duration-200"
>
	<div
		class="flex flex-col justify-center items-center sm:flex-row
			sm:justify-between"
	>
		<div
			class="w-1/2 sm:w-1/4 sm:h-auto sm:px-4 object-cover object-center"
		>
		</div>

		<div class="sm:flex-grow sm:px-4">
			<h1 data-test="welcomeTitle" class="text-5xl">
				K8cher Web
			</h1>

			<p class="text-xl mt-2">Should this be user dashboard?</p>

			<div class="mt-4">
				{#if $userStore.successfulLoad}
					<div
						data-test="greetMsg"
						class="px-5 py-3 bg-green-100 dark:bg-green-800 text-green-900 dark:text-green-100 transition-colors duration-200 rounded"
					>
						<span>You are logged in as user "{$userStore.user.email}".</span>

						{#each $userStore.roles as role}
							<h1>{role}</h1>
						{/each}

						<Button on:click="{setStore}">test set store</Button>
					</div>
				{:else}
					<div
						data-test="loginInstructions"
						class="px-5 py-3 bg-yellow-100 dark:bg-yellow-800 text-yellow-900 dark:text-yellow-100 rounded transition-colors duration-200"
					>
						<span>If you want to </span>
						<a class="font-semibold" href="/login">sign in</a><span
							>, you can try the default accounts:<br />-
							Administrator (login="admin" and password="admin")
							<br />- User (login="user" and password="user").</span
						>
					</div>
					<div
						class="px-5 py-3 mt-4 bg-yellow-100 dark:bg-yellow-800 text-yellow-900 dark:text-yellow-100 rounded transition-colors duration-200"
					>
						<span>You don't have an account yet?</span>&nbsp;
						<a
							data-test="svlRegisterHomeLink"
							class="font-semibold"
							href="/account/register">Register a new account</a
						>
					</div>
				{/if}
			</div>


		</div>
	</div>
</section>
