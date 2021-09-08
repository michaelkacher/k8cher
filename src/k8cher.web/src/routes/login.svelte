<script>
	import { goto } from '$app/navigation'

	// import appLogo from '$lib/svg/appLogo.svg'
	import Alert from '$lib/Alert.svelte'
	import Button from '$lib/Button.svelte'
	import InputControl from '$lib/InputControl.svelte'
	import CheckboxControl from '$lib/CheckboxControl.svelte'
	import EmailInput from '$lib/account/EmailInput.svelte';
	import { post } from '$lib/utils/apiHelper'
	import { userStore } from '$lib/auth/userStore.js'
	import { serverUrl } from '$lib/utils/env'
	import { page } from '$app/stores'

	let email
	let password
	let rememberMe = false
	let authError = false
	let validEmail = false
	let validPassword = false

	$: validForm = validEmail && validPassword

	async function login() {
		authError = false

		const res = await post(`${serverUrl}auth/login`, {
            email,
            password
        })

        if(res.success) {
			await userStore.loadFromJwt(res.json.jwt, rememberMe)
            goto('/')
        } else {
			authError = true
		}
	}

	$: showConfirm = $page && $page.query && $page.query.has('confirmation')
	
</script>

<svelte:head>
	<title>Sign in to K8cher Web</title>
	<meta name="Description" content="Sign in to K8cher Web" />
</svelte:head>

<section class="flex h-screen items-start justify-center">
	<div class="p-4 w-full sm:w-[450px] mx-4 sm:m-0">
		<div class="px-4 py-3 mt-4 sm:mx-0 flex justify-center">
			<div class="w-3/4 dark:text-primary-100">
			</div>
		</div>
		<div
			data-test="signInTitle"
			class="mt-4 px-4 w-full text-3xl text-center"
		>
			Sign in to K8cher Web
		</div>
		<Alert data-test="errorMsg" type="danger" show="{!!authError}">
			Incorrect username or password.
		</Alert>
		<Alert show="{showConfirm}" closeable="{false}">
			<span>Your user account has been activated.</span>
		</Alert>

		<form
			data-test="loginForm"
			class="mt-4 p-4 flex flex-col bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 shadow-md rounded"
		>
			<EmailInput
				value="{email}"
				on:input="{event => (email = event.detail.value)}"
				on:validate="{event => (validEmail = event.detail.valid)}"
			/>

			<InputControl
				id="password"
				type="password"
				label="Password"
				name="password"
				value="{password}"
				on:input="{event => (password = event.target.value)}"
				required
				validations="{[
					{ type: 'required', message: 'Password is mandatory' },
				]}"
				on:validate="{event => (validPassword = event.detail.valid)}"
			/>

			<div class="mt-1 mb-2 ml-1">
				<CheckboxControl
					name="rememberMe"
					checked="{rememberMe}"
					on:change="{event => (rememberMe = event.target.checked)}"
					>Remember me</CheckboxControl
				>
			</div>

			<Button type="submit" on:click="{login}" disabled="{!validForm}">
				Sign in
			</Button>
		</form>
		<div
			class="mt-5 px-4 flex justify-between text-primary-700 dark:text-primary-500"
		>
			<div>
				<a
					data-test="forgotPwdLink"
					href="/account/reset/init"
					class="font-semibold">Forgot password?</a
				>
			</div>
			<div>
				<a
					data-test="registerLink"
					href="/account/register"
					class="font-semibold">Create an account</a
				>
			</div>
		</div>
	</div>
</section>
