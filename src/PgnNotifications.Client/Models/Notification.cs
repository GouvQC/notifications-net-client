﻿using Newtonsoft.Json;

namespace PgnNotifications.Models
{
    public class Notification
    {
        public string id;
        [JsonProperty("completed_at")]
        public string completedAt;
        [JsonProperty("created_at")]
        public string createdAt;
        [JsonProperty("email_address")]
        public string emailAddress;
        public string body;
        public string subject;
        [JsonProperty("one_click_unsubscribe_url")]
        public string oneClickUnsubscribeURL;
        [JsonProperty("line_1")]
        public string line1;
        [JsonProperty("line_2")]
        public string line2;
        [JsonProperty("line_3")]
        public string line3;
        [JsonProperty("line_4")]
        public string line4;
        [JsonProperty("line_5")]
        public string line5;
        [JsonProperty("line_6")]
        public string line6;
        [JsonProperty("phone_number")]
        public string phoneNumber;
        public string postcode;
        public string postage;
        public string reference;
        [JsonProperty("sent_at")]
        public string sentAt;
        public string status;
        public Template template;
        public string type;
        [JsonProperty("created_by_name")]
        public string createdByName;
        [JsonProperty("is_cost_data_ready")]
        public bool isCostDataReady;
        [JsonProperty("cost_in_pounds")]
        public double? costInPounds;
        [JsonProperty("cost_details")]
        public CostDetails costDetails;


        public override bool Equals(object notification)
        {
            if (!(notification is Notification note))
            {
                return false;
            }

            return
                id == note.id &&
                completedAt == note.completedAt &&
                createdAt == note.createdAt &&
                emailAddress == note.emailAddress &&
                line1 == note.line1 &&
                line2 == note.line2 &&
                line3 == note.line3 &&
                line4 == note.line4 &&
                line5 == note.line5 &&
                line6 == note.line6 &&
                phoneNumber == note.phoneNumber &&
                postcode == note.postcode &&
                postage == note.postage &&
                reference == note.reference &&
                sentAt == note.sentAt &&
                status == note.status &&
                template.id == note.template.id &&
                template.uri == note.template.uri &&
                template.version == note.template.version &&
                type == note.type &&
                createdByName == note.createdByName &&
                oneClickUnsubscribeURL == note.oneClickUnsubscribeURL &&
                isCostDataReady == note.isCostDataReady &&
                costInPounds == note.costInPounds &&
                costDetails.billableSmsFragments == note.costDetails.billableSmsFragments &&
                costDetails.internationalRateMultiplier == note.costDetails.internationalRateMultiplier &&
                costDetails.smsRate == note.costDetails.smsRate &&
                costDetails.billableSheetsOfPaper == note.costDetails.billableSheetsOfPaper &&
                costDetails.postage == note.costDetails.postage;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
