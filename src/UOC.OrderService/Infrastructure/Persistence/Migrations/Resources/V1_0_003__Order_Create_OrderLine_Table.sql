CREATE TABLE "order"."order_line"
(
	id uuid PRIMARY KEY,
	order_id uuid NOT NULL,
	product_id uuid NOT NULL,
    amount decimal NOT NULL,
    create_date timestamp NOT NULL,
    modification_date timestamp NOT NULL,
    CONSTRAINT fk_order_orderline_id FOREIGN KEY(order_id) REFERENCES "order".order(id)
);



