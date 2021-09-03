<script>
	import Alert from "$lib/Alert.svelte";
	import Page from "$lib/page/Page.svelte";
	import RegisterUserForm from "$lib/account/RegisterUserForm.svelte";
	import { serverUrl } from "$lib/utils/env";
	import { post } from "$lib/utils/apiHelper";

	let username;
	let email;
	let password;

	let error;
	let accountCreated = false;

	async function createUserAccount() {
		const res = await post(
			`${serverUrl}auth/register`,
			{
				user: {
					email: email,
					userName: email,
				},
				password,
			}
		);

		accountCreated = res.success;
		if (res.message) {
			error = res.message;
		}
		// todo - mbk: probably want to change to
		// 1) enter e-mail
		// 2) send link
		// 3) on confirm screen set password
		//goto('/auth/confirmEmail')
	}

</script>

<svelte:head>
	<title>Create user account</title>
	<meta name="Description" content="Sign up - Create user account" />
</svelte:head>

<Page testId="register">
	<span slot="header">Create user account</span>
	<svelte:fragment slot="alerts">
		<Alert data-test="successMsg" show={accountCreated} closeable={false}>
			User account successfully created. Please check your email for
			confirmation.
		</Alert>
		<Alert data-test="errorMsg" type="danger" show={!!error}>
			<!-- todo - mbk: don't think I want to let people know if e-mail exists. 
				Instead read - "Finish registering from code sent to e-mail" -->
			{#if error}
				User account creation failed. Please try again later.
			{/if}
		</Alert>
	</svelte:fragment>
	{#if !accountCreated}
		<RegisterUserForm
			bind:username
			bind:email
			bind:password
			on:click={createUserAccount}
		/>
	{/if}
</Page>
