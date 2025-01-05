from uuid import UUID
from cachetools import LRUCache
import torch
from transformers import T5Tokenizer, T5ForConditionalGeneration
import pika
import json
import Request
import Response
from Queue import Queue

connection = pika.BlockingConnection(
    pika.ConnectionParameters(host='rabbitmq'))

channel = connection.channel()

channel.queue_declare(queue=Queue.AI_MODEL_QUEUE.value)

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

# Load the fine-tuned model and tokenizer
tokenizer = T5Tokenizer.from_pretrained("./t5_summarization")
model = T5ForConditionalGeneration.from_pretrained("./t5_summarization").to(device)

cache = LRUCache(maxsize=100)  # Adjust maxsize as per your use case

# Function to summarize text with caching
def summarize_text(dialogue):
    if dialogue in cache:
        print("Using cached result.")
        return cache[dialogue]
    
    print("Generating summary.")
    input_text = "summarize: " + dialogue
    inputs = tokenizer(input_text, return_tensors="pt", max_length=512, truncation=True).to(device)
    outputs = model.generate(inputs["input_ids"], max_length=128, num_beams=4, early_stopping=True)
    summary = tokenizer.decode(outputs[0], skip_special_tokens=True)
    
    # Store the result in cache
    cache[dialogue] = summary
    return summary

# Test the summarizer
test_dialogue = """
Traveler:
"This place... it feels like it’s been untouched for centuries. Does anyone else even know it exists?"
Shopkeeper:
"Many know of it, but few find it. The town chooses its visitors, not the other way around."
Traveler:
"That’s... cryptic. I was just following the trail up the ridge. One wrong turn, and I ended up here. So, what’s the secret? Why keep it hidden?"
Shopkeeper:
"Secrets, traveler, are what keep the heart alive. The moment everything is known, life loses its mystery. Are you sure you want answers?"
Traveler:
"Well, yeah. That’s why I’m asking! Why would anyone choose to live in the middle of nowhere, surrounded by mist and silence?"
Shopkeeper:
"Perhaps for the same reason you wander: to escape. Or to find something. Or someone."
Traveler:
"I’m not escaping. I just... needed a change of scenery."
Shopkeeper:
"Change always comes with a price. What are you willing to leave behind?"
Traveler:
"That depends on what I’m getting in return."
Shopkeeper:
"Ah, a barterer. In that case, step inside. I have something you might like... or fear."
"""

def on_request(ch, method, props, body):
    body_dict = json.loads(body)
    request = Request.Request(input=body_dict['Input'], user_id=UUID(body_dict['UserId']))
    summarized_text = summarize_text(request.input)
    response = {
        "UserId": user_id,
        "SummarizedText": summarized_text
    }
    print(props.reply_to)
    print(props.correlation_id)
    
    ch.basic_publish(exchange='',
                     routing_key=props.reply_to,
                     properties=pika.BasicProperties(correlation_id = \
                                                         props.correlation_id),
                     body=json.dumps(response))
    ch.basic_ack(delivery_tag=method.delivery_tag)

channel.basic_qos(prefetch_count=1)
channel.basic_consume(queue=Queue.AI_MODEL_QUEUE.value, on_message_callback=on_request)

print(" [x] Awaiting RPC requests")
channel.start_consuming()


