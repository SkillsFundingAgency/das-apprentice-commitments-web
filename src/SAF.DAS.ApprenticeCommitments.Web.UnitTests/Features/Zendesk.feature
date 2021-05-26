@outerApi

Feature: Zendesk
	In order to support apprentices the Zendesk widget is configured on every page

Scenario: Zendesk widget is configured
	When a page is requested
	Then the page contains the Zendesk configuration
