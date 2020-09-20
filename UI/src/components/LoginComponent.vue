<template>
	<div
		v-if="isShown"
		id="login"
		class="uk-background-secondary uk-light uk-position-center uk-padding"
	>
		<div :class="currentAnimation">
			<form>
				<fieldset class="uk-fieldset">
					<legend class="uk-legend uk-text-center">Logowanie</legend>

					<div class="uk-margin">
						<div class="uk-inline">
							<span
								class="uk-form-icon"
								uk-icon="icon: user"
							></span>
							<input
								v-model="login"
								class="uk-input"
								type="text"
								placeholder="Login"
							/>
						</div>
					</div>

					<div class="uk-margin uk-inline">
						<span class="uk-form-icon" uk-icon="icon: lock"></span>
						<input
							v-model="password"
							class="uk-input"
							type="password"
							placeholder="Hasło"
						/>
					</div>

					<div class="uk-margin">
						<button
							@click.prevent="sendRequest"
							class="uk-button uk-button-default uk-width-1-1"
						>
							Zaloguj się
						</button>
					</div>
				</fieldset>
			</form>
		</div>
	</div>
</template>

<script>
export default {
	name: 'LoginComponent',
	mounted: function() {
		window.emitter.on('showLoginWindow', () => {
			this.showWindow = true;
		});
		window.emitter.on('hideLoginWindow', () => {
			this.showWindow = false;
		});
	},
	data: function() {
		return {
			isShown: false,
			currentAnimation: '',
			login: '',
			password: '',
		};
	},
	methods: {
		sendRequest: function() {
			mp.trigger('OnLoginRequest', this.login, this.password);
		},
	},
	props: {
		showWindow: false,
	},
	watch: {
		showWindow: function(value) {
			if (value == this.isShown) return;
			if (value == true) {
				this.currentAnimation = 'uk-animation-scale-up';
				this.isShown = true;
				setTimeout(
					function(self) {
						self.currentAnimation = '';
					},
					500,
					this
				);
			} else {
				this.currentAnimation =
					'uk-animation-scale-up uk-animation-reverse';
				setTimeout(
					function(self) {
						self.isShown = false;
						self.currentAnimation = '';
					},
					500,
					this
				);
			}
		},
	},
};
</script>
