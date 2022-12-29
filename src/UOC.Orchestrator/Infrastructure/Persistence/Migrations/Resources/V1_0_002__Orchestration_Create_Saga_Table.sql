CREATE TABLE "orchestration"."order_payment_saga"
(
	correlation_id uuid PRIMARY KEY,
	order_id uuid NOT NULL,
    "state" varchar(50) NOT NULL,
    is_payment_rejected boolean NOT NULL,
    last_time_updated timestamp NOT NULL
);



