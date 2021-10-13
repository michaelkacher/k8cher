<script>
	import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons/faExclamationCircle.js'

	import InputControl from '$lib/InputControl.svelte'
	import Icon from '$lib/Icon.svelte'

	export let name = 'password'
	export let label = 'Password'
	export let value = ''
</script>

<InputControl
	type="password"
	label="{label}"
	value="{value}"
	name="{name}"
	on:input
	required
	validations="{[
		{ type: 'required', message: 'Password is mandatory' },
		{
			type: 'minlength',
			minlength: 4,
			message: 'Password is required to be at least 4 characters',
		},
		{
			type: 'maxlength',
			maxlength: 50,
			message: 'Password cannot be longer than 50 characters',
		},
		// Can remove any of these that the backend doesn't require.
		{
			type: 'pattern',
			pattern: /[a-z]/,
			message: 'Password must contain a lower-case letter',
		},
		{
			type: 'pattern',
			pattern: /[A-Z]/,
			message: 'Password must contain a capital letter',
		},
		{
			type: 'pattern',
			pattern: /[0-9]/,
			message: 'Password must contain a number',
		},
		{
			type: 'pattern',
			pattern: /[!@#$%^&*\(\)_\+\-\={}<>,\.\|""'~`:;\\?\/\[\] ]/,
			message: 'Password must contain a special character',
		},
	]}"
	on:validate
	let:message
	let:dirty
	let:valid
	{...$$restProps}
>
	<slot message="{message}" dirty="{dirty}" valid="{valid}">
		{#if dirty && !valid}
			<div data-test="{name}-error" class="flex items-center">
				<Icon classes="mr-2" icon="{faExclamationCircle}" />
				{message}
			</div>
		{:else}&nbsp;{/if}
	</slot>
</InputControl>
