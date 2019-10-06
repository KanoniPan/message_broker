import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  connection: HubConnection;
  messages: string[] = [];

  isReady: boolean;
  topic: string;

  ngOnInit() {
    this.connection = new HubConnectionBuilder()
      .withUrl('https://localhost:5001/api/chat-hub')
      .build();

    this.connection.on('connect', (messages: string) => {
      this.messages = JSON.parse(messages);
    });

    this.connection.on('message', (message: string) => {
      this.messages.push(message);
    });

    this.connection.on('error', (errorMessage: string) => {
      alert(errorMessage);
      this.connection = undefined;
    });

    this.connection.start()
      .then(() => this.isReady = true)
      .catch(console.log);
  }

  onTopicSubmit(topic: string) {
    this.isReady = false;

    this.connection.send('subscribe', topic)
      .then(() => {
        this.topic = topic;
        this.isReady = true;
      })
      .catch(console.log);
  }

  onMessageSubmit(message: string) {
    this.connection.send('sendMessage', message, this.topic).catch(console.log);
  }
}
