
* What do I want the actor store to be?
Potential experience:
* Create new actor
	* define json 'schema' for the actor?

potential functions:
Initialize
SetState
SetProperty
AddItemPropertyArray
RemoveItemPropertyArray
UpdateItemArray

* get error handling planned out and integrated (can it be tied to notifications in a slot?)
* Have register user feedback from server?

Account items to get working:
* or do I want to change flow first?
* Change password
* Reset password
* validate e-mail
	* setup mail slurper like item

* need to align minimum password with client/server (could this be done by using webassembly in each?). Just have server side validate and client side show message?

* the confirm password on register behaves differnt than password leaving wierd user experience when register fails
* Is Form.svelte even worth it, just use <form> directly?

* // todo - mbk: add a custom log that will log to console for 'dev' environment an option to send logs to server

* Add cypress and jest
https://www.cypress.io/
jest has jest.conf.cjs

* In app.html add manifest.json like hipster?

* In svelte.config.js look at adding proxy
vite: () => ({
			server: {
				proxy: {
					'/api': {
						target: 'http://localhost:8080',
						changeOrigin: true,
					},
					'/management': {
						target: 'http://localhost:8080',
						changeOrigin: true,
					},
				},
			},
		}),