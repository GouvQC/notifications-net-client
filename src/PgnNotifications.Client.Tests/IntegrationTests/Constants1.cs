using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace PgnNotifications.Client.Tests.IntegrationTests
{
    public static class Constants1
    {
        public static String fakeApiKey
        {
            get
            {
                return @"FAKEKEY-fd29e561-24b6-4f32-be5c-e642a1d68641-57bdfd56-ac07-409b-8307-71419d85bb9c";
            }
        }

        public static String fakeClientId
        {
            get
            {
                return @"FAKECLIENTID-f47ac10b-58cc-4372-a567-0e02b2c3d479";
            }
        }

        public static String fakePhoneNumber { get { return "07766565767"; } }
        public static String fakeEmail { get { return "test@mail.com"; } }
        public static String fakeName { get { return "test name"; } }
        public static String fakeSmsBulkName { get { return "Envoi groupé de test CSV avec phone numbers"; } }
        public static String fakeEmailBulkName { get { return "Envoi groupé de test Email avec emails et names"; } }
        public static String fakeNotificationId { get { return "902e6534-bc4a-4c87-8c3e-9f4144ca36fd"; } }
        public static String fakeNotificationReference { get { return "some-client-ref"; } }
        public static String fakeoneClickUnsubscribeURL { get { return "https://oneClickUnsubscribeURL.com/unsubscribe"; } }
        public static String fakeId { get { return "913e9fa6-9cbb-44ad-8f58-38487dccfd82"; } }
        public static String fakeTemplateId { get { return "913e9fa6-9cbb-44ad-8f58-38487dccfd82"; } }
        public static String fakeReplyToId { get { return "78ded4ad-e915-4a89-a314-2940ed141d40"; } }
        public static String fakeSMSSenderId { get { return "88ded4ad-e915-4a89-a314-2940ed141d41"; } }
        public const string fakeReference = "sample-test-ref";
        public const String TemplateIdSms = "mock-sms-template";
        public const String TemplateIdEmail = "mock-email-template";
        public const String SmsBody = "HELLO WORLD v2";
        public const String EmailBody = "HELLO WORLD";
        
        public static string fakeTemplateListJson = @"{
        ""templates"": [
            {
            ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
            ""body"": ""Fake Email"",
            ""subject"": ""Test Email Subject"",
            ""type"": ""email"",
            ""version"": 1
            }
        ]
        }";

        public const string fakeSmsBody = "Fake SMS";
        public const string fakeEmailBody = "Fake Email";
        public const string fakeEmailSubject = "Subject";

        public const string fakeEmailPreviewResponseJson = @"{
        ""id"": ""d683f7f9-df04-4b9c-8019-15092c4674fd"",
        ""type"": ""email"",
        ""version"": 2,
        ""body"": ""Fake Email"",
        ""subject"": ""Subject""
        }";

        public const string fakeSmsPreviewResponseJson = @"{
        ""id"": ""d683f7f9-df04-4b9c-8019-15092c4674fd"",
        ""type"": ""sms"",
        ""version"": 2,
        ""body"": ""Fake SMS"",
        ""subject"": null
        }";


        public static string fakeNotificationListJson = @"{
            ""notifications"": [
                {
                    ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                    ""status"": ""sending"",
                    ""template"": {
                        ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
                        ""version"": 1,
                        ""uri"": ""/service/fake/template""
                    },
                    ""phone_number"": ""+14185551234"",
                    ""created_at"": ""2024-01-01T00:00:00Z"",
                    ""type"": ""sms""
                }
            ]
        }";

        public static String fakeNotificationJson
        {
            get
            {
                return @"{
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""created_by_name"": ""A. Sender"",
                            ""email_address"": null,
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": ""+447588767647"",
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-22T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""sms"",
                            ""one_click_unsubscribe_url"": null,
                            ""is_cost_data_ready"": true,
                            ""cost_in_pounds"": 0.5,
                            ""cost_details"": {
                                ""billable_sms_fragments"": 1,
                                ""international_rate_multiplier"": 0.05,
                                ""sms_rate"": 0.5
                            }
                        }";
            }
        }

        public static String fakeNotificationsJson
        {
            get
            {
                return @"{
                    ""notifications"": [
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""created_by_name"": null,
                            ""email_address"": null,
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": ""+447588767647"",
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-22T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2
                            },
                            ""type"": ""sms"",
                            ""subject"": ""Test SMS Subject"",
                            ""body"": ""This is a test SMS body."",
                            ""one_click_unsubscribe_url"": null,
                            ""is_cost_data_ready"": true,
                            ""cost_in_pounds"": 0.5,
                            ""cost_details"": {
                                ""billable_sms_fragments"": 1,
                                ""international_rate_multiplier"": 0.05,
                                ""sms_rate"": 0.5
                            }
                        },
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-23T11:21:13.133522Z"",
                            ""created_by_name"": null,
                            ""email_address"": ""someone@example.com"",
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": null,
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-23T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd84"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2
                            },
                            ""type"": ""email"",
                            ""subject"": ""Test Email Subject"",
                            ""body"": ""This is a test email body."",
                            ""one_click_unsubscribe_url"": ""https://www.example.com/unsubscribe"",
                            ""is_cost_data_ready"": true,
                            ""cost_in_pounds"": 0.5,
                            ""cost_details"": {}
                        }
                    ],
                    ""links"": {
                        ""current"": ""http://example.com/notifications?page=1"",
                        ""next"": null
                    }
                }";
            }
        }


        public static String fakeSmsNotificationsJson
        {
            get
            {
                return @"{
                    ""notifications"": [
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""created_by_name"": null,
                            ""email_address"": null,
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": ""+447588767647"",
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-22T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""sms"",
                            ""one_click_unsubscribe_url"": null,
                            ""is_cost_data_ready"": true,
                            ""cost_in_pounds"": 0.5,
                            ""cost_details"": {
                                ""billable_sms_fragments"": 1,
                                ""international_rate_multiplier"": 0.05,
                                ""sms_rate"": 0.5
                            }
                        },
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-24T11:21:13.133522Z"",
                            ""created_by_name"": null,
                            ""email_address"": null,
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": ""+447588767647"",
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-24T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd84"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""sms"",
                            ""one_click_unsubscribe_url"": null,
                            ""is_cost_data_ready"": true,
                            ""cost_in_pounds"": 0.5,
                            ""cost_details"": {
                                ""billable_sms_fragments"": 1,
                                ""international_rate_multiplier"": 0.05,
                                ""sms_rate"": 0.5
                            }
                        }
                    ]
                }";
            }
        }

        public static String fakeEmailNotificationsJson
        {
            get
            {
                return @"{
                    ""notifications"": [
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""created_by_name"": null,
                            ""email_address"": ""someone@example.com"",
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": null,
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-22T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""email"",
                            ""one_click_unsubscribe_url"": ""https://www.example.com/unsubscribe"",
                            ""is_cost_data_ready"": true,
                            ""cost_in_pounds"": 0.0,
                            ""cost_details"": {}
                        },
                        {
                            ""completed_at"": null,
                            ""created_at"": ""2016-11-24T11:21:13.133522Z"",
                            ""created_by_name"": null,
                            ""email_address"": ""someone2@example.com"",
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""line_1"": null,
                            ""line_2"": null,
                            ""line_3"": null,
                            ""line_4"": null,
                            ""line_5"": null,
                            ""line_6"": null,
                            ""phone_number"": null,
                            ""postcode"": null,
                            ""postage"": null,
                            ""reference"": null,
                            ""sent_at"": ""2016-11-24T16:16:09.885808Z"",
                            ""status"": ""sending"",
                            ""template"": {
                                ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd84"",
                                ""uri"": ""/service/fd29e421-24b6-4f45-ac5c-e642a1d68641/template/323e9fa6-9cbb-44ad-8f64-38487dccfd43"",
                                ""version"": 2  },
                            ""type"": ""email"",
                            ""one_click_unsubscribe_url"": ""https://www.example.com/unsubscribe"",
                            ""is_cost_data_ready"": true,
                            ""cost_in_pounds"": 0.0,
                            ""cost_details"": {}
                        }
                    ]
                }";
            }
        }

        public static string fakeTemplateResponseJson => @"{
        ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
        ""body"": ""Fake Email"",
        ""subject"": ""Subject"",
        ""type"": ""email"",
        ""version"": 1
        }";

        public static String fakeTemplateListResponseJson
        {
            get
            {
                return @"{
                    ""templates"": [
                        {
                            ""updated_at"": null,
                            ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                            ""created_by"": ""someone@example.com"",
                            ""id"": ""902e4312-bc4a-4c87-8c3e-9f4144ca36fd"",
                            ""name"": ""SMS Template 1"",
                            ""body"": ""test body"",
                            ""subject"": null,
                            ""type"": ""sms"",
                            ""version"": 2
                        },
                        {
                            ""updated_at"": ""2016-12-23T11:21:13.133522Z"",
                            ""created_at"": ""2016-12-22T11:21:13.133522Z"",
                            ""created_by"": ""someoneelse@example.com"",
                            ""id"": ""902e4312-bc4a-4c86-8c3e-9f4144ca36fd"",
                            ""name"": ""Email Template 1"",
                            ""body"": ""test body 2"",
                            ""subject"": ""test subject 1"",
                            ""type"": ""email"",
                            ""version"": 3
                        }
                    ]
                }";
            }
        }


        public static String fakeReceivedTextListResponseJson
        {
            get
            {
                return @"{ ""received_text_messages"": [
                        {
                            ""user_number"": ""447700900111"",
                            ""created_at"": ""2017-11-02T15:07:57.197546Z"",
                            ""service_id"": ""a5149c32-f03b-4711-af49-ad6993797d45"",
                            ""id"": ""342786aa-23ce-4695-9aad-7f79e68ee29a"",
                            ""notify_number"": ""testing"",
                            ""content"": ""Hello""
                        },
                        {
                            ""user_number"": ""447700900111"",
                            ""created_at"": ""2017-11-02T15:07:57.197546Z"",
                            ""service_id"": ""a5149c32-f03b-4711-af49-ad6993797d45"",
                            ""id"": ""342786aa-23ce-4695-9aad-7f79e68ee29a"",
                            ""notify_number"": ""testing"",
                            ""content"": ""Hello again""
                        }
					]
				}";
            }
        }

        public static String fakeTemplateEmptyListResponseJson
        {
            get
            {
                return @"{ ""templates"": [] }";
            }
        }

        public const string fakeTemplateSmsListResponseJson = @"{
            ""templates"": [
                {
                    ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
                    ""name"": ""Fake SMS Template"",
                    ""body"": ""Fake SMS"",
                    ""subject"": null,
                    ""type"": ""sms"",
                    ""version"": 1,
                    ""created_at"": ""2016-11-22T11:21:13.133522Z"",
                    ""created_by"": ""test@example.com""
                }
            ]
        }";

        public static string fakeTemplateSmsResponseJson => @"
            {
            ""id"": ""913e9fa6-9cbb-44ad-8f58-38487dccfd82"",
            ""body"": ""Fake SMS"",
            ""subject"": null,
            ""type"": ""sms"",
            ""version"": 1
            }";


        public static String fakeTemplateEmailListResponseJson
        {
            get
            {
                return @"{ ""templates"": [{
                            ""updated_at"": ""2016-12-23T11:21:13.133522Z"",
                            ""created_at"": ""2016-12-22T11:21:13.133522Z"",
                            ""created_by"": ""someone@email.com"",
                            ""id"": ""902e4312-bc4a-4c86-8c3e-9f4144ca36fd"",
                            ""body"": ""test body 2"",
                            ""subject"": ""test subject 2"",
                            ""type"": ""email"",
                            ""version"": 3
                        }]}";
            }
        }

        public static String fakeCsvBulkJson
        {
            get
            {
                return
                        $@"phone_number,name
                        {fakePhoneNumber},{fakeName}
                        {fakePhoneNumber},{fakeName}";
            }
        }


        public static List<List<string>> fakeRowsEmailBulk
        {
            get
            {
                return new List<List<string>>
                {
                    new List<string> { "email_address", "name" },
                    new List<string> { fakePhoneNumber, fakeName },
                    new List<string> { fakePhoneNumber, fakeName }
                };
                        }
        }

        public static String fakeTemplatePreviewResponseJson
        {
            get
            {
                return @"{
                            ""id"": ""d683f7f9-df04-4b9c-8019-15092c4674fd"",
                            ""type"": ""sms"",
                            ""version"": 2,
                            ""body"": ""test body"",
                            ""subject"": null
                         }";
            }
        }

        public static string fakeEmailBulkResponseJson
        {
            get
            {
                var rowsJson = JArray.FromObject(fakeRowsEmailBulk).ToString(Formatting.None);

                return $@"{{
                    ""id"":  ""{fakeId}"",
                    ""template_id"": ""{fakeTemplateId}"",
                    ""name"": ""{fakeEmailBulkName}"",
                    ""rows"": {rowsJson},
                    ""reference"": ""{fakeNotificationReference}"",
                    ""replyToId"": ""{fakeReplyToId}""
                }}";
            }
        }
        
        public static string fakeSmsBulkResponseJson
        {
            get
            {
                var csvJson = JsonConvert.ToString(fakeCsvBulkJson);

                return $@"{{
                    ""id"":  ""{fakeId}"",
                    ""template_id"": ""{fakeTemplateId}"",
                    ""name"": ""{fakeSmsBulkName}"",
                    ""csv"": {csvJson},
                    ""reference"": ""{fakeNotificationReference}"",
                    ""replyToId"": ""{fakeReplyToId}""
                }}";
            }
        }

        public static string fakeSmsNotificationResponseJson
        {
            get
            {
                return @"{
                    ""id"": ""d683f7f9-df04-4b9c-8019-15092c4674fd"",
                    ""reference"": ""sample-test-ref"",
                    ""content"": {
                        ""body"": ""Fake SMS"",
                        ""from_number"": null
                    },
                    ""template"": {
                        ""id"": ""be35a391-e912-42e9-82e6-3f4953f6cbb0"",
                        ""uri"": ""http://someurl/v2/templates/be35a391-e912-42e9-82e6-3f4953f6cbb0"",
                        ""version"": 1
                    },
                    ""uri"": ""http://some_url/v2/notifications/d683f7f9-df04-4b9c-8019-15092c4674fd""
                }";
            }
        }

        public static String fakeSmsNotificationWithSMSSenderIdResponseJson
        {
            get
            {
                return @"{
                            ""content"": {
                                ""body"": ""test"",
                                ""from_number"": ""PGN"" },
                            ""id"": ""d683f7f9-df04-4b9c-8019-15092c4674fd"",
                            ""reference"":  null,
                            ""template"": {
                                ""id"": ""be35a391-e912-42e9-82e6-3f4953f6cbb0"",
                                ""uri"": ""http://someurl/v2/templates/be35a391-e912-42e9-82e6-3f4953f6cbb0"",
                                ""version"": 1 },
                            ""uri"": ""http://some_url//v2/notifications/d683f7f9-df04-4b9c-8019-15092c4674fd""
                         }";
            }
        }

        public static String fakeEmailNotificationResponseJson
        {
            get
            {
                return @"{
                            ""content"": {
                                ""body"": ""Fake Email"",
                                ""from_email"": ""someone@mail.com"",
                                ""subject"": ""Subject"",
                                ""one_click_unsubscribe_url"": null
                            },
                            ""id"": ""731b9c83-563f-4b59-afc5-87e9ca717833"",
                            ""reference"":  ""sample-test-ref"",
                            ""template"": {
                                ""id"": ""f0bb62f7-5ddb-4bf8-aac7-7ey6aefd1524"",
                                ""uri"": ""https://someurl/v2/templates/c0bs62f7-4ddb-6bf8-cac7-c1e6aefd1524"",
                                ""version"": 5
                            },
                            ""uri"": ""https://someurl//v2/notifications/321b9c43-563f-4c59-sac5-87e9ca325833""
                        }";
            }
        }
    }
}
