<script>
	import Button from '$lib/Button.svelte'

	import Form from '$lib/page/Form.svelte'
	import PasswordConfirm from '$lib/account/PasswordConfirm.svelte'
	import EmailInput from '$lib/account/EmailInput.svelte';

	export let email = ''
	export let password = ''
	let validEmail = false
	let validPassword = false

	$: validForm = validEmail && validPassword
</script>

<Form testId="register">

	<EmailInput
		value="{email}"
		on:input="{event => (email = event.detail.value)}"
		on:validate="{event => (validEmail = event.detail.valid)}"
	/>

	<PasswordConfirm
		label="Password"
		value="{password}"
		on:input="{event => (password = event.detail.value)}"
		on:validate="{event => (validPassword = event.detail.valid)}"
	/>

	<Button type="submit" on:click disabled="{!validForm}">
		Create account
	</Button>
</Form>
