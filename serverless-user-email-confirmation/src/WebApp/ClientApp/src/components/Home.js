import React, { Component } from 'react';

export class Home extends Component {

    constructor(props) {
        super(props);
        this.state = {
            name: "",
            email: "",
            password: "",
            isProcessingRegistration: false,
            registrationProcessStatusUrl: "",
            intervalId: 0,
            statusMessage: "",
            registrationConfirmationURL: "",
            registrationProcessResult: ""
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }



    handleChange(event) {
        this.setState({
            [event.target.name]: event.target.value
        });
    }

    startRegistrationValidation() {
        var request = {
            method: 'post',
            headers: {
                'Accept': 'application/json, text/plain, */*',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email: this.state.email, password: this.state.password, name: this.state.name })
        };
        fetch('http://localhost:7071/api/UserRegistration', request)
            .then(function (response) { return response.json() })
            .then(data => {
                if (this.state.intervalId)
                    clearInterval(this.state.intervalId);


                var intervalId = setInterval(() => {
                    this.checkValidationStatus();
                }, 5000);

                this.setState({ registrationProcessStatusUrl: data.statusQueryGetUri, intervalId: intervalId });
            });
    }

    handleSubmit(event) {
        this.setState({
            isProcessingRegistration: true,
            statusMessage: "Processing...",
            registrationProcessResult: ""
        });
        event.preventDefault();
        this.startRegistrationValidation();

    }

    checkValidationStatus() {
        if (this.state.registrationProcessStatusUrl) {
            fetch(this.state.registrationProcessStatusUrl)
                .then(function (response) { return response.json() })
                .then(data => {
                    if (data.runtimeStatus === "Completed") {
                        clearInterval(this.state.intervalId);
                        this.setState({ intervalId: 0, statusMessage: data.customStatus.text, isProcessingRegistration: false, name: "", password: "", email: "", registrationConfirmationURL: "", registrationProcessResult: data.customStatus.text });
                    } else {
                        if (data.customStatus)
                            this.setState({ statusMessage: data.customStatus.text, registrationConfirmationURL: data.customStatus.registrationConfirmationURL });
                    }
                });
        }
    }

    componentWillUnmount() {
        // use intervalId from the state to clear the interval
        if (this.state.intervalId)
            clearInterval(this.state.intervalId);
    }

    render() {
        if (this.state.isProcessingRegistration) {
            return (
                <div>
                    <h1>Registration Example</h1>
                    <p>You will receive an email once you click register. Clicking on the "confirm registration" email will unblock the user creation process</p>
                    <div>Status URL: <a href={this.state.registrationProcessStatusUrl} target="_blank">{this.state.registrationProcessStatusUrl}</a></div>
                    <div><a href={this.state.registrationConfirmationURL} target="_blank" disabled={this.state.registrationConfirmationURL.length === 0}>DEBUG: Confirm by clicking here</a></div>
                    <div>Current Status: <strong>{this.state.statusMessage}</strong></div>
                </div>
            );
        }
        else if (this.state.registrationProcessResult.length > 0) {
            return (
                <div>
                    <h1>Registration Example</h1>
                    <p>You will receive an email once you click register. Clicking on the "confirm registration" email will unblock the user creation process</p>
                    <div>Status URL: <a href={this.state.registrationProcessStatusUrl} target="_blank">{this.state.registrationProcessStatusUrl}</a></div>
                    <div>Final Status: <strong>{this.state.statusMessage}</strong></div>
                </div>
            );
        }
        else {
            return (
                <div>
                    <h1>Registration Example</h1>
                    <p>You will receive an email once you click register. Clicking on the "confirm registration" email will unblock the user creation process</p>
                    <form role="form" onSubmit={this.handleSubmit}>
                        <fieldset>
                            <div className="form-group">
                                <input className="form-control" onChange={this.handleChange} placeholder="Name" name="name" type="text" />
                            </div>
                            <div className="form-group">
                                <input className="form-control" onChange={this.handleChange} placeholder="E-mail" name="email" type="text" />
                            </div>
                            <div className="form-group">
                                <input className="form-control" onChange={this.handleChange} placeholder="Password" name="password" type="password" value="" />
                            </div>
                            <input className="btn btn-lg btn-success btn-block" type="submit" disabled={this.state.isProcessingRegistration} value="Register" />
                        </fieldset>
                    </form>
                </div>
            );
        }
    }
}
