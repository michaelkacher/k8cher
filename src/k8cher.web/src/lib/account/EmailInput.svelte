<script>
	import InputControl from '$lib/InputControl.svelte'
	import { createEventDispatcher } from 'svelte'

	let value = ''
	let validEmail = false
	const dispatch = createEventDispatcher()

	function handleEmail(event) {
		value = event.target.value
		dispatch('input', { value })
	}

	function handleEmailValidation(event) {
		validEmail = event.detail.valid
		dispatch('validate', { valid: validEmail })
	}
</script>

<InputControl
	type="email"
	label="Email"
	name="email"
	value="{value}"
	on:input="{handleEmail}"
	required
	validations="{[
		{ type: 'required', message: 'Email is mandatory' },
		{
			type: 'minlength',
			minlength: 5,
			message: 'Email is required to be at least 5 characters',
		},
		{
			type: 'maxlength',
			maxlength: 254,
			message: 'Email cannot be longer than 254 characters',
		},
		{
			type: 'pattern',
			pattern:
				/^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
			message: 'Email address is not valid',
		},
	]}"
	on:validate="{handleEmailValidation}"
/>
