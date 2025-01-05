from enum import Enum, unique

@unique
class Queue(Enum):
    USER_QUEUE = "userQueue"
    AI_MODEL_QUEUE = "aiModelQueue"

    @property
    def description(self):
        """Return the description of the enum value."""
        return self.value
