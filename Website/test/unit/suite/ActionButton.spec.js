import expect from '../helpers/expect.js';
import sinon from 'sinon';
import React from 'react';
import TestUtils from 'react-addons-test-utils';
import ActionButton from '../../../Composite/console/ActionButton.js';

describe('ActionButton', () => {
	let renderer, props, state;
	beforeEach(() => {
		renderer = TestUtils.createRenderer();
		state = { state: true };
		props = {
			label: "Label",
			getState: sinon.spy(() => state),
			action: sinon.spy()
		}
		renderer.render(
			<ActionButton {...props}/>
		)
	});

	it('should render a button', () => {
		return expect(renderer, 'to have rendered',
			<button>{props.label}</button>
		);
	});

	it('should get state when clicked', () => {
		return expect(renderer, 'with event', 'click')
			.then(() => {
				return expect(props.getState, 'was called');
			});
	});

	it('should call handler with state contents when clicked', () => {
		return expect(renderer, 'with event', 'click')
			.then(() => {
				return expect(props.action, 'was called with', state);
			});
	});
})
