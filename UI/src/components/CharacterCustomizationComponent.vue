<template>
	<div
		v-if="isShown"
		id="characterCustomization"
		class="uk-background-secondary uk-light uk-position-left uk-padding"
	>
		<div :class="currentAnimation">
			<legend class="uk-legend uk-text-center">Wygląd postaci</legend>
			<form class="uk-form-stacked">
				<div>
					<div v-if="gender == 0">
						Mężczyzna
					</div>
					<div v-else>
						Kobieta
					</div>
					<input
						class="uk-range"
						type="range"
						min="0"
						max="1"
						v-model="gender"
						v-on:change="updateGender($event)"
					/>
				</div>
				<div v-for="faceOption in faceOptions">
					<label class="uk-form-label">
						{{ faceOption.name }}({{ faceOption.value }})
					</label>
					<div class="uk-form-controls">
						<input
							class="uk-range"
							type="range"
							:min="faceOption.min"
							:max="faceOption.max"
							step="0.01"
							v-model="faceOption.value"
							v-on:input="
								updateFaceOption(faceOption.index, $event)
							"
						/>
					</div>
				</div>
				<div v-for="clothOption in clothOptions">
					<label class="uk-form-label">
						{{ clothOption.name }}({{ clothOption.value }})
					</label>
					<div class="uk-form-controls">
						<div v-if="gender == 0">
							<input
								class="uk-range"
								type="range"
								:min="clothOption.maleMin"
								:max="clothOption.maleMax"
								step="1"
								v-model="clothOption.value"
								v-on:input="
									updateClothOption(clothOption.index, $event)
								"
							/>
						</div>
						<div v-else>
							<input
								class="uk-range"
								type="range"
								:min="clothOption.femaleMin"
								:max="clothOption.femaleMax"
								step="1"
								v-model="clothOption.value"
								v-on:input="
									updateClothOption(clothOption.index, $event)
								"
							/>
						</div>
					</div>
				</div>
				<div v-for="propOption in propOptions">
					<label class="uk-form-label">
						{{ propOption.name }}({{ propOption.value }})
					</label>
					<div class="uk-form-controls">
						<div v-if="gender == 0">
							<input
								class="uk-range"
								type="range"
								:min="propOption.maleMin"
								:max="propOption.maleMax"
								step="1"
								v-model="propOption.value"
								v-on:input="
									updatePropOption(propOption.index, $event)
								"
							/>
						</div>
						<div v-else>
							<input
								class="uk-range"
								type="range"
								:min="propOption.femaleMin"
								:max="propOption.femaleMax"
								step="1"
								v-model="propOption.value"
								v-on:input="
									updatePropOption(propOption.index, $event)
								"
							/>
						</div>
					</div>
				</div>
				<button
					@click.prevent="saveCustomization"
					class="uk-button uk-button-default uk-width-1-1"
				>
					Zapisz
				</button>
			</form>
		</div>
	</div>
</template>

<script>
export default {
	name: 'CharacterCustomizationComponent',
	mounted: function() {
		window.emitter.on('showCharacterCustomizationWindow', () => {
			this.showWindow = true;
		});
		window.emitter.on('hideCharacterCustomizationWindow', () => {
			this.showWindow = false;
		});
		window.emitter.on(
			'characterCustomizationLoaded',
			(gender, faceOptions, clothOptions, propOptions) => {
				this.gender = gender;
				this.faceOptions = faceOptions;
				this.clothOptions = clothOptions;
				this.propOptions = propOptions;
			}
		);
	},
	data: function() {
		return {
			isShown: false,
			currentAnimation: '',
			gender: 0,
			faceOptions: null,
			clothOptions: null,
			propOptions: null,
		};
	},
	methods: {
		updateGender: function(event) {
			var gender = 'male';

			if (event.target.value == 1) {
				gender = 'female';
			}
			mp.trigger('UpdateGender', gender);
		},
		updateFaceOption: function(index, event) {
			mp.trigger(
				'UpdateFaceOption',
				parseInt(index),
				parseFloat(event.target.value)
			);
		},
		updateClothOption: function(index, event) {
			mp.trigger(
				'UpdateClothOption',
				parseInt(index),
				parseInt(event.target.value)
			);
		},
		updatePropOption: function(index, event) {
			mp.trigger(
				'UpdatePropOption',
				parseInt(index),
				parseInt(event.target.value)
			);
		},
		saveCustomization: function() {
			var genderToSend = 'male';

			if (this.gender == 1) {
				genderToSend = 'female';
			}

			mp.trigger(
				'SaveCustomization',
				genderToSend,
				JSON.stringify(this.faceOptions),
				JSON.stringify(this.clothOptions),
				JSON.stringify(this.propOptions)
			);
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
