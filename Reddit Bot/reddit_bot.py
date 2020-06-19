import praw
import time
import os
import json
import re
import random


REPLY_MESSAGE = ("Hi, [Here](https://imgur.com/r/aww/i72rdxt) is "
                 "something you would like")


def authenticate():
    print('Authenticating .....')
    reddit = praw.Reddit(
        'bot',
        user_agent="kvsqashit's joke comment responder v0.1")
    print('Authenticated as {}'.format(reddit.user.me()))
    print('logged in.')
    return reddit


def get_saved_comments():
    if not os.path.isfile('comments_replied_to.txt'):
        comments_replied_to = []
    else:
        with open('comments_replied_to.txt', 'r') as f:
            comments_replied_to = f.read()
            comments_replied_to = comments_replied_to.split('\n')
            comments_replied_to = list(filter(None, comments_replied_to))

    return comments_replied_to


def get_random_quote():
    with open('quotes.json', 'r') as quotes:
        bot_quotes = json.load(quotes)
    return random.choice(bot_quotes)


comments_replied_to = get_saved_comments()
print(comments_replied_to)


def is_trigger_mentioned(text):
    with open("triggers.json", 'r') as triggers:
        triggers_list = json.load(triggers)

    for trigger in triggers_list:
        # Do a case insentive search
        if re.search(trigger, text, re.IGNORECASE):
            return True
    return False


def main():
    reddit = authenticate()
    while True:
        run_bot(reddit, comments_replied_to)


def run_bot(reddit, comments_replied_to):
    print('fetching 10 comments ....')
    for comment in reddit.subreddit('test').comments(limit=10):
        # print(comment.author.name + " :   " + comment.body)
        if (
            is_trigger_mentioned(comment.body) and
            comment.id not in comments_replied_to and
            not comment.author == reddit.user.me()
        ):
            print(comment.author.name + " : " + comment.body)
            # comment.reply(REPLY_MESSAGE)
            comment.reply(get_random_quote())
            comments_replied_to.append(comment.id)
            print('replied to comment ' + comment.id)
            with open('comments_replied_to.txt', 'a') as f:
                f.write(comment.id + '\n')

    print(comments_replied_to)
    print("sleeping for 5 seconds....")
    time.sleep(10)


if __name__ == '__main__':
    main()
